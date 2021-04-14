using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FMODUnity;
using System;
using RunicSounds.EngineWrapper;

namespace RunicSounds {
    namespace RSEditor {

        [CustomPropertyDrawer(typeof(PersistentFMODParamField))]
        public class PersistentFMODParamFieldDrawer : PropertyDrawer {

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
                EditorGUI.BeginProperty(position, label, property);

                SerializedProperty paramRefStringProperty = property.FindPropertyRelative("stringFMODRef");
                SerializedProperty paramIDPropertyd1 = property.FindPropertyRelative("paramdata1");
                SerializedProperty paramIDPropertyd2 = property.FindPropertyRelative("paramdata2");

                EditorParamRef paramRef = EventManager.Parameters.Find((x) =>
                    ((paramIDPropertyd1.longValue) == x.ID.data1 && (paramIDPropertyd2.longValue) == x.ID.data2));

                if (paramRef != null) {
                    paramRefStringProperty.stringValue = paramRef.Name;
                }
                else {
                    if (paramIDPropertyd1.longValue == 0 && paramIDPropertyd2.longValue == 0) {
                        if (EventManager.Parameters.Count > 0) {
                            paramRefStringProperty.stringValue = EventManager.Parameters[0].Name;
                            paramIDPropertyd1.longValue = EventManager.Parameters[0].ID.data1;
                            paramIDPropertyd2.longValue = EventManager.Parameters[0].ID.data2;
                        }
                    }
                    else {
                        Debug.Log("wrong data? " + paramIDPropertyd1.longValue + "." + paramIDPropertyd2.longValue);
                    }
                }

                List<string> parameterOptions = new List<string>();
                for (int i = 0; i < EventManager.Parameters.Count; i++) {
                    parameterOptions.Add(EventManager.Parameters[i].Name);
                }

                parameterOptions.Sort((lhs, rhs) => lhs.CompareTo(rhs));

                int choiceIndex = 0;
                for (int i = 0; i < parameterOptions.Count; i++) {
                    if (parameterOptions[i] == paramRefStringProperty.stringValue) {
                        choiceIndex = i;
                    }
                }

                int originalIndex = choiceIndex;

                choiceIndex = EditorGUI.Popup(position, choiceIndex, parameterOptions.ToArray());

                if (choiceIndex != originalIndex) {
                    EditorParamRef newParamRef = EventManager.Parameters.Find((x) => x.Name == parameterOptions[choiceIndex]);
                    paramRefStringProperty.stringValue = newParamRef.Name;
                    paramIDPropertyd1.longValue = newParamRef.ID.data1;
                    paramIDPropertyd2.longValue = newParamRef.ID.data2;
                }

                EditorGUI.EndProperty();
            }
        }
    }
}