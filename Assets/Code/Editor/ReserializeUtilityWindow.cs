using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


// reserialization utility - useful for e.g., forcing prefabs to update after updating unity editor for the project
public sealed class ReserializeUtilityWindow : EditorWindow
{
    [SerializeField] private List<DefaultAsset> folders = new();
    [SerializeField] private AssetQuery.AssetKinds kinds = AssetQuery.AssetKinds.Prefabs;

    [Header("Run mode")]
    [SerializeField] private ReserializePipeline.RunMode mode = ReserializePipeline.RunMode.Reserialize;

    [Header("Script dependency filter")]
    [SerializeField] private bool onlyAssetsThatReferenceScript = false;
    [SerializeField] private MonoScript filterScript = null;

    [Header("Logging / diff control")]
    [SerializeField] private bool onlyLogActuallyChanged = true;
    [SerializeField] private bool includeMetaInHash = false;

    [Header("Performance")]
    [SerializeField] private bool useFastYamlReferenceScan = true;
    [SerializeField] private bool fallbackToGetDependenciesForNonYaml = false;

    private ReserializePipeline.CancelToken cancelToken;
    private bool operationInProgress = false;

    [MenuItem("Tools/Serialization/Reserialize Utility...")]
    private static void Open()
    {
        var w = GetWindow<ReserializeUtilityWindow>("Reserialize Utility");
        w.minSize = new Vector2(560, 390);
        w.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Targets", EditorStyles.boldLabel);
        kinds = (AssetQuery.AssetKinds)EditorGUILayout.EnumFlagsField("Asset kinds", kinds);

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Run mode", EditorStyles.boldLabel);
        mode = (ReserializePipeline.RunMode)EditorGUILayout.EnumPopup("Mode", mode);

        if (mode == ReserializePipeline.RunMode.DryRun)
        {
            EditorGUILayout.HelpBox(
                "Dry Run only lists which assets would be reserialized (clickable). It does not write files.",
                MessageType.Info);
        }

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Folders (optional)", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "If folders are provided, only assets under those folders are included.\n" +
            "If empty, it scans under Assets/.",
            MessageType.Info);

        DrawFolderList();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Script filter", EditorStyles.boldLabel);

        onlyAssetsThatReferenceScript = EditorGUILayout.ToggleLeft(
            "Only include assets that reference this script",
            onlyAssetsThatReferenceScript);

        using (new EditorGUI.DisabledScope(!onlyAssetsThatReferenceScript))
        {
            filterScript = (MonoScript)EditorGUILayout.ObjectField(
                "Script", filterScript, typeof(MonoScript), false);

            useFastYamlReferenceScan = EditorGUILayout.ToggleLeft(
                "Use fast YAML reference scan (recommended)", useFastYamlReferenceScan);

            using (new EditorGUI.DisabledScope(useFastYamlReferenceScan))
            {
                fallbackToGetDependenciesForNonYaml = EditorGUILayout.ToggleLeft(
                    "Fallback to AssetDatabase.GetDependencies (slower)",
                    fallbackToGetDependenciesForNonYaml);
            }
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Logging / diff control", EditorStyles.boldLabel);

        using (new EditorGUI.DisabledScope(mode == ReserializePipeline.RunMode.DryRun))
        {
            onlyLogActuallyChanged = EditorGUILayout.ToggleLeft(
                "Only log assets whose file contents actually changed (hash)",
                onlyLogActuallyChanged);

            includeMetaInHash = EditorGUILayout.ToggleLeft(
                "Include .meta in hash (more accurate, slightly slower)",
                includeMetaInHash);
        }

        EditorGUILayout.Space(14);

        var settings = BuildSettings();
        if (!ReserializePipeline.TryValidate(settings, out var validationError) && !string.IsNullOrEmpty(validationError))
        {
            EditorGUILayout.HelpBox(validationError, MessageType.Warning);
        }

        using (new EditorGUI.DisabledScope(!ReserializePipeline.TryValidate(settings, out _)))
        {
            if (GUILayout.Button(mode == ReserializePipeline.RunMode.DryRun ? "Dry Run" : "Force Reserialize", GUILayout.Height(36)))
                Run(settings);
        }

        using (new EditorGUI.DisabledScope(!operationInProgress))
        {
            if (GUILayout.Button("Stop (cancel)", GUILayout.Height(26)))
                cancelToken?.Cancel();
        }
    }

    private void DrawFolderList()
    {
        int removeIndex = -1;

        for (int i = 0; i < folders.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            folders[i] = (DefaultAsset)EditorGUILayout.ObjectField(folders[i], typeof(DefaultAsset), false);
            if (GUILayout.Button("X", GUILayout.Width(24))) removeIndex = i;
            EditorGUILayout.EndHorizontal();
        }

        if (removeIndex >= 0) folders.RemoveAt(removeIndex);

        EditorGUILayout.Space(4);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Folder")) folders.Add(null);
        if (GUILayout.Button("Clear")) folders.Clear();
        EditorGUILayout.EndHorizontal();
    }

    private ReserializePipeline.Settings BuildSettings() => new(
        rootFolders: ResolveFolderPaths(),
        kinds: kinds,
        onlyAssetsThatReferenceScript: onlyAssetsThatReferenceScript,
        filterScript: filterScript,
        onlyLogActuallyChanged: onlyLogActuallyChanged,
        includeMetaInHash: includeMetaInHash,
        useFastYamlReferenceScan: useFastYamlReferenceScan,
        fallbackToGetDependenciesForNonYaml: fallbackToGetDependenciesForNonYaml,
        mode: mode);

    private void Run(ReserializePipeline.Settings settings)
    {
        cancelToken = new ReserializePipeline.CancelToken();
        operationInProgress = true;

        try
        {
            ReserializePipeline.TryRun(settings, cancelToken, out var result);
            ReserializePipeline.LogResult(result);
        }
        finally
        {
            operationInProgress = false;
            cancelToken = null;
        }
    }

    private List<string> ResolveFolderPaths()
    {
        var paths = new List<string>();
        foreach (var f in folders)
        {
            if (f == null) continue;
            var p = AssetDatabase.GetAssetPath(f);
            if (!string.IsNullOrEmpty(p) && AssetDatabase.IsValidFolder(p))
                paths.Add(p);
        }

        return paths;
    }
}
