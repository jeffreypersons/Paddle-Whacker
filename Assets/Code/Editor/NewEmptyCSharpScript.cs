using UnityEditor;
using UnityEngine;
using System.Linq;

public static class ReserializeTools
{
    [MenuItem("Tools/Serialization/Force Reserialize Selected")]
    private static void ForceReserializeSelected()
    {
        var paths = Selection.assetGUIDs
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(p => !string.IsNullOrEmpty(p))
            .ToList();

        if (paths.Count == 0)
        {
            Debug.LogWarning("No assets selected.");
            return;
        }

        AssetDatabase.ForceReserializeAssets(paths);
        AssetDatabase.SaveAssets();
        Debug.Log($"Reserialized {paths.Count} assets.");
    }

    [MenuItem("Tools/Serialization/Force Reserialize All Prefabs")]
    private static void ForceReserializeAllPrefabs()
    {
        var guids = AssetDatabase.FindAssets("t:Prefab");
        var paths = guids
            .Select(AssetDatabase.GUIDToAssetPath)
            .ToList();

        AssetDatabase.ForceReserializeAssets(paths);
        AssetDatabase.SaveAssets();

        Debug.Log($"Reserialized {paths.Count} prefabs.");
    }
}