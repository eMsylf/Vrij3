using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RunicSounds {
    namespace RSEditor {

        [CustomPropertyDrawer(typeof(AudioBank))]
        public class AudioBankDrawer : PropertyDrawer {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
                EditorGUI.BeginProperty(position, label, property);

                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

                EditorGUI.PropertyField(position, property.FindPropertyRelative("bankReference"), GUIContent.none);

                EditorGUI.EndProperty();
            }
        }
    }
}
