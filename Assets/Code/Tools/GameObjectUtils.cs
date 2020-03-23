using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public static class GameObjectUtils
{
    public static void SetAlpha(SpriteRenderer renderer, float alphaLevel)
    {
        Color color = renderer.color;
        renderer.color = new Color(color.r, color.g, color.b, alphaLevel);
    }
    public static void SetButtonVisibility(Button button, bool isVisible)
    {
        button.gameObject.SetActive(isVisible);
        button.enabled = isVisible;
    }
    public static void SetLabelVisibility(TMPro.TextMeshProUGUI label, bool isVisible)
    {
        label.enabled = isVisible;
    }
    // note: only fetches active gameObjects
    public static List<GameObject> FindAllObjectsWithTags(params string[] tags)
    {
        var objects = new List<GameObject>();
        foreach (string tag in tags)
        {
            objects.AddRange(GameObject.FindGameObjectsWithTag(tag));
        }
        return objects;
    }
    // note: only fetches active gameObjects
    public static List<T> FindAllObjectsWithTags<T>(params string[] tags) where T : Object
    {
        var objects = new List<T>();
        foreach (string tag in tags)
        {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
            for (int i = 0; i < objectsWithTag.Length; i++)
            {
                T component;
                if (objectsWithTag[i].TryGetComponent(out component))
                {
                    objects.Add(component);
                }
            }
        }
        return objects;
    }
}
