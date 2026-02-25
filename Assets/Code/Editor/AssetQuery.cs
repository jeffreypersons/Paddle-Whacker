using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


public sealed class AssetQuery
{
    [Flags]
    public enum AssetKinds
    {
        Prefabs = 1 << 0,
        Scenes = 1 << 1,
        ScriptableObjects = 1 << 2,
        Materials = 1 << 3,
        Animations = 1 << 4,
        AllSupported = Prefabs | Scenes | ScriptableObjects | Materials | Animations
    }

    public class CancelToken { public bool IsCancellationRequested; public void Cancel() => IsCancellationRequested = true; }

    public readonly struct Spec
    {
        public readonly List<string> Roots;
        public readonly AssetKinds Kinds;
        public readonly bool RequireScriptReference;
        public readonly string ScriptGuid;
        public readonly bool UseFastYamlGuidScan;
        public readonly bool FallbackToGetDependenciesForNonYaml;

        public Spec(List<string> roots, AssetKinds kinds, bool requireScriptReference, string scriptGuid, bool useFastYamlGuidScan, bool fallbackToGetDependenciesForNonYaml)
        {
            Roots = roots ?? new List<string>();
            Kinds = kinds;
            RequireScriptReference = requireScriptReference;
            ScriptGuid = scriptGuid;
            UseFastYamlGuidScan = useFastYamlGuidScan;
            FallbackToGetDependenciesForNonYaml = fallbackToGetDependenciesForNonYaml;
        }
    }

    private readonly List<string> roots = new();
    private AssetKinds kinds = AssetKinds.Prefabs;
    private bool requireScriptRef = false;
    private MonoScript script = null;
    private string scriptGuid = null;
    private bool fastYamlGuidScan = true;
    private bool fallbackDepsForNonYaml = false;

    public static string ToFullPath(string assetPath)
    {
        var projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
        return Path.GetFullPath(Path.Combine(projectRoot, assetPath));
    }

    public AssetQuery Under(IEnumerable<string> roots)
    {
        this.roots.Clear();
        if (roots != null)
            this.roots.AddRange(roots.Where(r => !string.IsNullOrEmpty(r)));
        return this;
    }

    public AssetQuery AddRoot(string root)
    {
        if (!string.IsNullOrEmpty(root)) roots.Add(root);
        return this;
    }

    public AssetQuery Kinds(AssetKinds kinds) { this.kinds = kinds; return this; }

    public AssetQuery ReferencingScript(MonoScript script)
    {
        requireScriptRef = true;
        this.script = script;
        if (!TryBuildScriptGuid(script, out scriptGuid, out var err))
            throw new ArgumentException(err);
        return this;
    }

    public AssetQuery NoScriptFilter() { requireScriptRef = false; script = null; scriptGuid = null; return this; }
    public AssetQuery UseFastYamlScan(bool enabled) { fastYamlGuidScan = enabled; return this; }
    public AssetQuery FallbackToDependenciesForNonYaml(bool enabled) { fallbackDepsForNonYaml = enabled; return this; }

    public Spec Build() => new Spec(new List<string>(roots), kinds, requireScriptRef, scriptGuid, fastYamlGuidScan, fallbackDepsForNonYaml);

    public bool TryValidate(out string error) => TryValidate(Build(), out error);

    public static bool TryValidate(in Spec spec, out string error)
    {
        if (spec.RequireScriptReference && string.IsNullOrEmpty(spec.ScriptGuid))
        {
            error = "Script filter is enabled but ScriptGuid is empty.";
            return false;
        }

        var invalidRoots = spec.Roots?.Where(r => !string.IsNullOrEmpty(r) && !AssetDatabase.IsValidFolder(r)).ToList();
        if (invalidRoots?.Count > 0)
        {
            error = $"Invalid folder path(s): '{string.Join(", ", invalidRoots)}'";
            return false;
        }

        error = null;
        return true;
    }

    // Execute pipeline
    public static List<string> Execute(in Spec spec, CancelToken cancel = null)
    {
        cancel ??= new CancelToken();
        if (!TryValidate(spec, out _)) return new List<string>();

        var roots = NormalizeRoots(spec.Roots);
        var candidates = FindByKinds(roots, spec.Kinds);

        if (!spec.RequireScriptReference || string.IsNullOrEmpty(spec.ScriptGuid))
            return candidates;

        return spec.UseFastYamlGuidScan
            ? FilterByGuidYamlScan(candidates, spec.ScriptGuid, spec.FallbackToGetDependenciesForNonYaml, cancel)
            : FilterByGuidDependencies(candidates, spec.ScriptGuid, cancel);
    }

    public static bool TryBuildScriptGuid(MonoScript script, out string guid, out string error)
    {
        if (script == null) { guid = null; error = "Script was null."; return false; }
        var path = AssetDatabase.GetAssetPath(script);
        if (string.IsNullOrEmpty(path)) { guid = null; error = "Script path was invalid."; return false; }
        guid = AssetDatabase.AssetPathToGUID(path);
        if (string.IsNullOrEmpty(guid)) { error = "Could not resolve GUID for script."; return false; }
        error = null;
        return true;
    }

    private static List<string> NormalizeRoots(List<string> roots)
    {
        if (roots == null || roots.Count == 0) return new List<string> { "Assets" };
        var valid = roots.Where(r => !string.IsNullOrEmpty(r) && AssetDatabase.IsValidFolder(r)).ToList();
        return valid.Count == 0 ? new List<string> { "Assets" } : valid;
    }

    private static List<string> FindByKinds(List<string> roots, AssetKinds kinds)
    {
        var filters = BuildTypeFilters(kinds);
        var rootArray = roots.ToArray();
        return filters
            .SelectMany(f => AssetDatabase.FindAssets(f, rootArray))
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(IsProjectAssetPath)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(p => p, StringComparer.Ordinal)
            .ToList();
    }

    private static List<string> BuildTypeFilters(AssetKinds kinds)
    {
        var filters = new List<string>();
        if (kinds.HasFlag(AssetKinds.Prefabs)) filters.Add("t:Prefab");
        if (kinds.HasFlag(AssetKinds.Scenes)) filters.Add("t:Scene");
        if (kinds.HasFlag(AssetKinds.ScriptableObjects)) filters.Add("t:ScriptableObject");
        if (kinds.HasFlag(AssetKinds.Materials)) filters.Add("t:Material");
        if (kinds.HasFlag(AssetKinds.Animations)) filters.Add("t:AnimationClip");
        if (filters.Count == 0) filters.Add("t:Prefab");
        return filters;
    }

    private static bool IsProjectAssetPath(string path) => !string.IsNullOrEmpty(path) && (path == "Assets" || path.StartsWith("Assets/", StringComparison.Ordinal));

    // --- Filter helpers ---
    private static List<string> FilterByGuidYamlScan(List<string> candidates, string guid, bool fallbackDepsForNonYaml, CancelToken cancel)
    {
        var needle = $"guid: {guid}";
        var result = new List<string>(Math.Max(8, candidates.Count / 4));

        foreach (var path in candidates)
        {
            if (cancel.IsCancellationRequested) break;
            if (LooksLikeTextSerializedAsset(path))
            {
                if (FileContainsSubstring(path, needle)) result.Add(path);
            }
            else if (fallbackDepsForNonYaml && DependsOnGuidViaDependencies(path, guid))
                result.Add(path);
        }
        return result;
    }

    private static List<string> FilterByGuidDependencies(List<string> candidates, string guid, CancelToken cancel)
    {
        var result = new List<string>(Math.Max(8, candidates.Count / 4));
        foreach (var path in candidates)
        {
            if (cancel.IsCancellationRequested) break;
            if (DependsOnGuidViaDependencies(path, guid)) result.Add(path);
        }
        return result;
    }

    private static bool DependsOnGuidViaDependencies(string assetPath, string guid)
    {
        var deps = AssetDatabase.GetDependencies(assetPath, true);
        return deps.Any(d => AssetDatabase.AssetPathToGUID(d) == guid);
    }

    private static bool LooksLikeTextSerializedAsset(string assetPath)
    {
        var ext = Path.GetExtension(assetPath).ToLowerInvariant();
        return ext is ".prefab" or ".unity" or ".asset" or ".mat" or ".anim" or ".controller" or ".overridecontroller";
    }

    private static bool FileContainsSubstring(string assetPath, string needle)
    {
        var fullPath = ToFullPath(assetPath);
        if (!File.Exists(fullPath)) return false;

        const int bufSize = 64 * 1024;
        var buffer = new char[bufSize];
        var carry = string.Empty;

        using var sr = new StreamReader(fullPath, Encoding.UTF8, true);
        while (!sr.EndOfStream)
        {
            int read = sr.Read(buffer, 0, buffer.Length);
            if (read <= 0) break;

            var chunk = carry + new string(buffer, 0, read);
            if (chunk.Contains(needle, StringComparison.Ordinal)) return true;

            carry = chunk.Substring(chunk.Length - Math.Min(needle.Length + 32, chunk.Length));
        }
        return false;
    }
}
