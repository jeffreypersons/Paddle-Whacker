using System.Collections.Generic;
using UnityEngine;


// notes:
// - all these methods use old style for loops so that the found value CAN be modified after return
// - as typical in unity api, that aside from the topmost parent, children of inactive parents are _not_ searched
public static class ObjectUtils
{
    private static bool IsMatchingCriteria(Transform transform, bool includeInactive, params string[] tags)
    {
        if (transform == null || (includeInactive || (!includeInactive && transform.gameObject.activeSelf)))
        {
            return false;
        }

        foreach (string tag in tags)
        {
            if (transform.CompareTag(tag))
            {
                return true;
            }
        }
        return false;
    }

    public static GameObject GetChildWithTag(GameObject parent, string tag, bool includeInactive=false)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            Transform child = parent.transform.GetChild(i);
            if (IsMatchingCriteria(child, includeInactive, tag))
            {
                return child.gameObject;
            }
        }
        return default;
    }
    // return FIRST found child matching given tag and component type
    public static T GetComponentInChildWithTag<T>(GameObject parent, string tag, bool includeInactive=false)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            Transform child = parent.transform.GetChild(i);
            if (IsMatchingCriteria(child, includeInactive, tag) && child.TryGetComponent(out T component))
            {
                return component;
            }
        }
        return default;
    }


    // return ALL found children matching given tag and component type
    public static List<GameObject> GetChildrenWithTags(GameObject parent, string[] tags, bool includeInactive=false)
    {
        var objects = new List<GameObject>();
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            Transform child = parent.transform.GetChild(i);
            if (IsMatchingCriteria(child, includeInactive, tags))
            {
                objects.Add(child.gameObject);
            }
        }
        return objects;
    }
    // return ALL found child matching given tag and component type (note: any parent components are NOT included)
    public static List<T> GetComponentsInChildrenWithTags<T>(GameObject parent, string[] tags, bool includeInactive=false)
    {
        var components = new List<T>();
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            Transform child = parent.transform.GetChild(i);
            if (IsMatchingCriteria(child, includeInactive, tags) && child.TryGetComponent(out T component))
            {
                components.Add(component);
            }
        }
        return components;
    }
}
