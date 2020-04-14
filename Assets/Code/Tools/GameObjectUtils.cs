using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


// note that these methods use old style for loops so that the found value an be modified after return
public static class GameObjectUtils
{
    public static void AddAutoUnsubscribeOnClickListenerToButton(Button button, System.Action onButtonClicked)
    {
        // note null initialization is required to force nonlocal scope of the handler, see https://stackoverflow.com/a/1362244
        UnityAction handler = null;
        handler = () =>
        {
            button.onClick.RemoveListener(handler);
            onButtonClicked.Invoke();
        };
        button.onClick.AddListener(handler);
    }
    public static void DisableButtonCompletely(Button button)
    {
        button.enabled = false;
        button.gameObject.SetActive(false);
    }

    // hides button WITHOUT disabling/setting-inactive
    public static void SetButtonVisibility(Button button, bool isVisible)
    {
        button.image.enabled = isVisible;
        button.enabled = isVisible;
    }
    public static void SetLabelVisibility(TMPro.TextMeshProUGUI label, bool isVisible)
    {
        label.enabled = isVisible;
    }
    public static void SetSpriteVisibility(SpriteRenderer spriteRenderer, bool isVisible)
    {
        spriteRenderer.enabled = isVisible;
    }
    public static void SetAlpha(SpriteRenderer renderer, float alphaLevel)
    {
        Color color = renderer.color;
        renderer.color = new Color(color.r, color.g, color.b, alphaLevel);
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
    // note: only fetches active game objects, null if not found
    public static GameObject FindFirstChildWithTag(GameObject gameObject, string tag)
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Transform child = gameObject.transform.GetChild(i);
            if (child.CompareTag(tag))
            {
                return child.gameObject;
            }
        }
        return default;
    }
    // note: only fetches active game objects, null if not found
    public static T FindFirstChildWithTag<T>(GameObject gameObject, string tag)
    {
        T component;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Transform child = gameObject.transform.GetChild(i);
            if (child.CompareTag(tag) && child.TryGetComponent(out component))
            {
                return component;
            }
        }
        return default;
    }
}
