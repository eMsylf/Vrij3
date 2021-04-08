using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FMODUnity;

namespace RunicSounds {

    [CustomPropertyDrawer(typeof(AudioEvent))]
    public class AudioEventDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            EditorGUI.PropertyField(position, property.FindPropertyRelative("persistentFMODField"), GUIContent.none);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            SerializedProperty fmodStringProperty = property.FindPropertyRelative("persistentFMODField").FindPropertyRelative("stringFMODRef");
            bool expanded = fmodStringProperty.isExpanded && !string.IsNullOrEmpty(fmodStringProperty.stringValue) && EventManager.EventFromPath(fmodStringProperty.stringValue) != null;
            float baseHeight = GUI.skin.textField.CalcSize(new GUIContent()).y;
            return baseHeight * (expanded ? 7 : 2); // 6 lines of info
        }
    }
}