using UnityEditor;
using UnityEngine;

namespace BobJeltes.Attributes
{
    public class InlinePropertyDrawer : PropertyDrawer
    {
        public static readonly GUIContent[] labels = { 
            new GUIContent("a"),
            new GUIContent("b"),
            new GUIContent("c"),
            new GUIContent("d"),
        };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Debug.Log("Hallo??");
            EditorGUI.MultiPropertyField(position, labels, property.GetArrayElementAtIndex(0));
        }
    }
}