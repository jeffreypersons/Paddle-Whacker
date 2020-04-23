using UnityEngine;
using UnityEditor;
using System;


// adopted from: http://www.brechtos.com/tagselectorattribute/
[CustomPropertyDrawer(typeof(TagSelectorAttribute))]
public class TagSelectorPropertyDrawer : PropertyDrawer
{
    private string[] tags;
    private const string EMPTY_TAG_NAME = "<No Tag>";

    private void UpdateTags()
    {
        tags = CollectionUtils.PrependToArray(EMPTY_TAG_NAME, UnityEditorInternal.InternalEditorUtility.tags);
    }

    private int IndexOfTag(string tag)
    {
        return tag == "" ? 0 : Array.IndexOf(tags, tag, 1);
    }

    // get the latest tags from editor and display them in a dropdown with our drawer getting the selected tag
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType == SerializedPropertyType.String)
        {
            EditorGUI.BeginProperty(position, label, property);

            var attrib = this.attribute as TagSelectorAttribute;
            if (attrib.UseDefaultTagFieldDrawer)
            {
                property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
            }
            else
            {
                UpdateTags();
                int selectedIndex = EditorGUI.Popup(position, label.text, IndexOfTag(property.stringValue), displayedOptions: tags);
                property.stringValue = selectedIndex >= 1? tags[selectedIndex] : "";
            }

            EditorGUI.EndProperty();
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }
    }
}
