using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;


public static class ReserializePipeline
{
    public enum RunMode
    {
        Reserialize,
        DryRun
    }

    public readonly struct Settings
    {
        public readonly List<string> RootFolders;
        public readonly AssetQuery.AssetKinds Kinds;
        public readonly RunMode Mode;
        public readonly bool OnlyAssetsThatReferenceScript;
        public readonly MonoScript FilterScript;
        public readonly bool UseFastYamlReferenceScan;
        public readonly bool FallbackToGetDependenciesForNonYaml;
        public readonly bool OnlyLogActuallyChanged;
        public readonly bool IncludeMetaInHash;

        public Settings(List<string> rootFolders, AssetQuery.AssetKinds kinds, RunMode mode,
            bool onlyAssetsThatReferenceScript, MonoScript filterScript,
            bool useFastYamlReferenceScan, bool fallbackToGetDependenciesForNonYaml,
            bool onlyLogActuallyChanged, bool includeMetaInHash)
        {
            RootFolders = rootFolders ?? new List<string>();
            Kinds = kinds;
            Mode = mode;
            OnlyAssetsThatReferenceScript = onlyAssetsThatReferenceScript;
            FilterScript = filterScript;
            UseFastYamlReferenceScan = useFastYamlReferenceScan;
            FallbackToGetDependenciesForNonYaml = fallbackToGetDependenciesForNonYaml;
            OnlyLogActuallyChanged = onlyLogActuallyChanged;
            IncludeMetaInHash = includeMetaInHash;
        }
    }

    public sealed class CancelToken : AssetQuery.CancelToken { }

    public readonly struct Result
    {
        public readonly RunMode Mode;
        public readonly bool WasCanceled;
        public readonly int MatchedCount;
        public readonly int ProcessedCount;
        public readonly List<string> MatchedPaths;
        public readonly List<string> ChangedPaths;

        public Result(RunMode mode, bool wasCanceled, int matchedCount, int processedCount,
            List<string> matchedPaths, List<string> changedPaths)
        {
            Mode = mode;
            WasCanceled = wasCanceled;
            MatchedCount = matchedCount;
            ProcessedCount = processedCount;
            MatchedPaths = matchedPaths ?? new List<string>();
            ChangedPaths = changedPaths ?? new List<string>();
        }
    }

    public static bool TryValidate(Settings s, out string error)
    {
        if (s.RootFolders != null)
        {
            foreach (var r in s.RootFolders)
            {
                if (string.IsNullOrEmpty(r)) continue;
                if (!AssetDatabase.IsValidFolder(r))
                {
                    error = $"Invalid folder path: '{r}'. (Expected something like 'Assets/YourFolder')";
                    return false;
                }
            }
        }

        if (s.OnlyAssetsThatReferenceScript)
        {
            if (!AssetQuery.TryBuildScriptGuid(s.FilterScript, out _, out error))
                return false;
        }

        error = null;
        return true;
    }

    public static bool TryRun(Settings s, CancelToken cancel, out Result result)
    {
        cancel ??= new CancelToken();

        if (!TryValidate(s, out var validationError))
        {
            Debug.LogWarning(validationError);
            result = new Result(s.Mode, false, 0, 0, new List<string>(), new List<string>());
            return false;
        }

        try
        {
            // Build the query
            string scriptGuid = null;
            if (s.OnlyAssetsThatReferenceScript)
                AssetQuery.TryBuildScriptGuid(s.FilterScript, out scriptGuid, out _);

            var spec = new AssetQuery.Spec(
                roots: s.RootFolders,
                kinds: s.Kinds,
                requireScriptReference: s.OnlyAssetsThatReferenceScript,
                scriptGuid: scriptGuid,
                useFastYamlGuidScan: s.UseFastYamlReferenceScan,
                fallbackToGetDependenciesForNonYaml: s.FallbackToGetDependenciesForNonYaml);

            var matched = AssetQuery.Execute(spec, cancel);

            if (matched.Count == 0)
            {
                Debug.Log("No assets matched the query.");
                result = new Result(s.Mode, cancel.IsCancellationRequested, 0, 0, matched, new List<string>());
                return false;
            }

            int processed = matched.Count;

            // Dry Run
            if (s.Mode == RunMode.DryRun)
            {
                result = new Result(RunMode.DryRun, cancel.IsCancellationRequested, matched.Count, processed, matched, new List<string>());
                return !cancel.IsCancellationRequested;
            }

            // Hash before (optional)
            Dictionary<string, string> before = null;
            if (s.OnlyLogActuallyChanged)
            {
                before = new Dictionary<string, string>(matched.Count);
                using var progress = new ProgressScope("Hashing (before)", cancel);
                processed = 0;

                foreach (var path in matched)
                {
                    if (progress.Step(path, processed, matched.Count)) break;
                    before[path] = HashAssetAndMaybeMeta(path, s.IncludeMetaInHash);
                    processed++;
                }

                if (cancel.IsCancellationRequested)
                {
                    result = new Result(RunMode.Reserialize, true, matched.Count, processed, matched, new List<string>());
                    return false;
                }
            }

            // Reserialize
            AssetDatabase.ForceReserializeAssets(matched);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Hash after + diff
            List<string> changed;
            if (!s.OnlyLogActuallyChanged)
            {
                changed = matched;
            }
            else
            {
                changed = new List<string>(Math.Max(8, matched.Count / 4));
                using var progress = new ProgressScope("Hashing (after)", cancel);

                for (int i = 0; i < matched.Count; i++)
                {
                    if (progress.Step(matched[i], i, matched.Count)) break;

                    var after = HashAssetAndMaybeMeta(matched[i], s.IncludeMetaInHash);
                    if (!string.Equals(before[matched[i]], after, StringComparison.Ordinal))
                        changed.Add(matched[i]);
                }
            }

            result = new Result(RunMode.Reserialize, cancel.IsCancellationRequested, matched.Count, processed, matched, changed);
            return !cancel.IsCancellationRequested;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Reserialize failed:\n{ex}");
            result = new Result(s.Mode, false, 0, 0, new List<string>(), new List<string>());
            return false;
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    public static void LogResult(Result r)
    {
        if (r.Mode == RunMode.DryRun)
        {
            Debug.Log(r.WasCanceled
                ? $"Dry run canceled. Matched {r.MatchedCount} assets."
                : $"Dry run complete. Matched {r.MatchedCount} assets.");

            foreach (var p in r.MatchedPaths)
                LogClickable(p, "Would reserialize");

            return;
        }

        if (r.WasCanceled)
        {
            Debug.Log($"Reserialize canceled. Matched {r.MatchedCount} assets; processed {r.ProcessedCount}. Changed list may be incomplete ({r.ChangedPaths.Count}).");
            foreach (var p in r.ChangedPaths)
                LogClickable(p, "Reserialized (changed, partial)");
            return;
        }

        if (r.ChangedPaths.Count == 0)
        {
            Debug.Log($"Reserialize complete. Matched {r.MatchedCount} assets; processed {r.ProcessedCount}; no file-content changes detected.");
            return;
        }

        Debug.Log($"Reserialize complete. Matched {r.MatchedCount} assets; processed {r.ProcessedCount}; {r.ChangedPaths.Count} changed:");
        foreach (var p in r.ChangedPaths)
            LogClickable(p, "Reserialized (changed)");
    }

    private static void LogClickable(string assetPath, string prefix)
    {
        var ctx = AssetDatabase.LoadMainAssetAtPath(assetPath);
        Debug.Log($"{prefix}: {assetPath}", ctx);
    }

    private static string HashAssetAndMaybeMeta(string assetPath, bool includeMeta)
    {
        using var sha = SHA1.Create();
        var sb = new StringBuilder(64);

        void AddHash(string fullPath)
        {
            sb.Append('|');
            if (!File.Exists(fullPath))
            {
                sb.Append("MISSING");
                return;
            }

            using var fs = File.OpenRead(fullPath);
            var bytes = sha.ComputeHash(fs);
            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));
        }

        AddHash(AssetQuery.ToFullPath(assetPath));
        if (includeMeta) AddHash(AssetQuery.ToFullPath(assetPath) + ".meta");
        return sb.ToString();
    }

    private readonly struct ProgressScope : IDisposable
    {
        private readonly string title;
        private readonly AssetQuery.CancelToken cancel;

        public ProgressScope(string title, AssetQuery.CancelToken cancel)
        {
            this.title = title;
            this.cancel = cancel ?? new AssetQuery.CancelToken();
        }

        public bool Step(string info, int index, int total)
        {
            if (cancel.IsCancellationRequested) return true;

            bool canceled = EditorUtility.DisplayCancelableProgressBar(
                title,
                $"{index}/{total}  {info}",
                total <= 0 ? 1f : (float)index / total);

            if (canceled) cancel.Cancel();
            return cancel.IsCancellationRequested;
        }

        public void Dispose() => EditorUtility.ClearProgressBar();
    }
}
