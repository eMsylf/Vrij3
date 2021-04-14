using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.IO;
using FMODUnity;
using RunicSounds.EngineWrapper;

namespace RunicSounds {
    namespace RSEditor {

        [CustomPropertyDrawer(typeof(PersistentFMODEventField))]
        class PersistentFMODEventFieldDrawer : PropertyDrawer {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
                SerializedProperty fmodStringProperty = property.FindPropertyRelative("stringFMODRef");
                SerializedProperty fmodGUIDProperty = property.FindPropertyRelative("fmodGUID");

                byte[] fmodguid = new byte[16];
                bool isGUIDPropertyValid = fmodGUIDProperty.arraySize == 16;

                if (isGUIDPropertyValid == false) {
                    fmodGUIDProperty.arraySize = 16;
                    for (int i = 0; i < 16; i++) {
                        fmodGUIDProperty.GetArrayElementAtIndex(i).intValue = 0;
                    }
                    fmodGUIDProperty.serializedObject.ApplyModifiedProperties();
                    return;
                }

                for (int i = 0; i < 16; i++) {
                    if (fmodGUIDProperty.arraySize != 16) {
                        fmodguid[i] = 0;
                    } else {
                        fmodguid[i] = (byte)fmodGUIDProperty.GetArrayElementAtIndex(i).intValue;
                    }
                }

                var oldFmodEvent = EventManager.EventFromGUID(new System.Guid(fmodguid));
                if (oldFmodEvent != null) {
                    fmodStringProperty.stringValue = oldFmodEvent.Path;
                }


                Texture browseIcon = EditorGUIUtility.Load("FMOD/SearchIconBlack.png") as Texture;
                Texture openIcon = EditorGUIUtility.Load("FMOD/BrowserIcon.png") as Texture;
                Texture addIcon = EditorGUIUtility.Load("FMOD/AddIcon.png") as Texture;

                label = EditorGUI.BeginProperty(position, label, fmodStringProperty);
                SerializedProperty pathProperty = fmodStringProperty;

                Event e = Event.current;
                if (e.type == EventType.DragPerform && position.Contains(e.mousePosition)) {
                    if (DragAndDrop.objectReferences.Length > 0 &&
                        DragAndDrop.objectReferences[0] != null &&
                        DragAndDrop.objectReferences[0].GetType() == typeof(EditorEventRef)) {
                        pathProperty.stringValue = ((EditorEventRef)DragAndDrop.objectReferences[0]).Path;
                        GUI.changed = true;
                        e.Use();
                    }
                }
                if (e.type == EventType.DragUpdated && position.Contains(e.mousePosition)) {
                    if (DragAndDrop.objectReferences.Length > 0 &&
                        DragAndDrop.objectReferences[0] != null &&
                        DragAndDrop.objectReferences[0].GetType() == typeof(EditorEventRef)) {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                        DragAndDrop.AcceptDrag();
                        e.Use();
                    }
                }

                float baseHeight = GUI.skin.textField.CalcSize(new GUIContent()).y;

                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.padding.top = 1;
                buttonStyle.padding.bottom = 1;

                Rect addRect = new Rect(position.x + position.width - addIcon.width - 7, position.y, addIcon.width + 7, baseHeight);
                Rect openRect = new Rect(addRect.x - openIcon.width - 7, position.y, openIcon.width + 6, baseHeight);
                Rect searchRect = new Rect(openRect.x - browseIcon.width - 9, position.y, browseIcon.width + 8, baseHeight);
                Rect pathRect = new Rect(position.x, position.y, searchRect.x - position.x - 3, baseHeight);

                var result = GUI.TextField(pathRect, pathProperty.stringValue);
                //EditorGUI.PropertyField(pathRect, pathProperty, GUIContent.none);

                if (result != pathProperty.stringValue) {
                    pathProperty.stringValue = result;
                    property.serializedObject.ApplyModifiedProperties();
                    OnEventSet(property);
                    return;
                }

                if (GUI.Button(searchRect, new GUIContent(browseIcon, "Search"), buttonStyle)) {
                    var eventBrowser = ScriptableObject.CreateInstance<EventBrowser>();

                    eventBrowser.ChooseEvent(fmodStringProperty, () => OnEventSet(property));
                    var windowRect = position;
                    windowRect.position = GUIUtility.GUIToScreenPoint(windowRect.position);
                    windowRect.height = openRect.height + 1;
                    eventBrowser.ShowAsDropDown(windowRect, new Vector2(windowRect.width, 400));

                }
                if (GUI.Button(addRect, new GUIContent(addIcon, "Create New Event in Studio"), buttonStyle)) {
                    var addDropdown = EditorWindow.CreateInstance<CreateEventPopup>();

                    addDropdown.SelectEvent(fmodStringProperty, fmodGUIDProperty);
                    var windowRect = position;
                    windowRect.position = GUIUtility.GUIToScreenPoint(windowRect.position);
                    windowRect.height = openRect.height + 1;
                    addDropdown.ShowAsDropDown(windowRect, new Vector2(windowRect.width, 500));

                }
                if (GUI.Button(openRect, new GUIContent(openIcon, "Open In Browser"), buttonStyle) &&
                    !string.IsNullOrEmpty(pathProperty.stringValue) &&
                    EventManager.EventFromPath(pathProperty.stringValue) != null
                    ) {
                    EventBrowser.ShowWindow();
                    EventBrowser eventBrowser = EditorWindow.GetWindow<EventBrowser>();
                    eventBrowser.FrameEvent(pathProperty.stringValue);
                }

                if (!string.IsNullOrEmpty(pathProperty.stringValue) && EventManager.EventFromPath(pathProperty.stringValue) != null) {
                    Rect foldoutRect = new Rect(position.x + 10, position.y + baseHeight, position.width, baseHeight);
                    fmodStringProperty.isExpanded = EditorGUI.Foldout(foldoutRect, fmodStringProperty.isExpanded, "Event Properties");
                    if (fmodStringProperty.isExpanded) {
                        var style = new GUIStyle(GUI.skin.label);
                        style.richText = true;
                        EditorEventRef eventRef = EventManager.EventFromPath(pathProperty.stringValue);
                        float width = style.CalcSize(new GUIContent("<b>Oneshot</b>")).x;
                        Rect labelRect = new Rect(position.x, position.y + baseHeight * 2, width, baseHeight);
                        Rect valueRect = new Rect(position.x + width + 10, position.y + baseHeight * 2, pathRect.width, baseHeight);

                        if (pathProperty.stringValue.StartsWith("{")) {
                            GUI.Label(labelRect, new GUIContent("<b>Path</b>"), style);
                            EditorGUI.SelectableLabel(valueRect, eventRef.Path);
                        }
                        else {
                            GUI.Label(labelRect, new GUIContent("<b>GUID</b>"), style);
                            EditorGUI.SelectableLabel(valueRect, eventRef.Guid.ToString("b"));
                        }
                        labelRect.y += baseHeight;
                        valueRect.y += baseHeight;

                        GUI.Label(labelRect, new GUIContent("<b>Banks</b>"), style);
                        GUI.Label(valueRect, string.Join(", ", eventRef.Banks.Select(x => x.Name).ToArray()));
                        labelRect.y += baseHeight;
                        valueRect.y += baseHeight;

                        GUI.Label(labelRect, new GUIContent("<b>Panning</b>"), style);
                        GUI.Label(valueRect, eventRef.Is3D ? "3D" : "2D");
                        labelRect.y += baseHeight;
                        valueRect.y += baseHeight;

                        GUI.Label(labelRect, new GUIContent("<b>Stream</b>"), style);
                        GUI.Label(valueRect, eventRef.IsStream.ToString());
                        labelRect.y += baseHeight;
                        valueRect.y += baseHeight;

                        GUI.Label(labelRect, new GUIContent("<b>Oneshot</b>"), style);
                        GUI.Label(valueRect, eventRef.IsOneShot.ToString());
                        labelRect.y += baseHeight;
                        valueRect.y += baseHeight;
                    }
                }
                else {
                    Rect labelRect = new Rect(position.x, position.y + baseHeight, position.width, baseHeight);
                    GUI.Label(labelRect, new GUIContent("Event Not Found", EditorGUIUtility.Load("FMOD/NotFound.png") as Texture2D));
                }

                EditorGUI.EndProperty();
            }

            private void OnEventSet(SerializedProperty property) {
                property.serializedObject.Update();

                byte[] fmodguid = new byte[16];

                for (int i = 0; i < 16; i++) {
                    fmodguid[i] = 0;
                }

                SerializedProperty fmodStringProperty = property.FindPropertyRelative("stringFMODRef");
                SerializedProperty fmodGUIDProperty = property.FindPropertyRelative("fmodGUID");

                var newFmodEvent = EventManager.EventFromPath(fmodStringProperty.stringValue);

                if (newFmodEvent != null) {
                    Array.Copy(newFmodEvent.Guid.ToByteArray(), fmodguid, 16);
                }

                for (int i = 0; i < 16; i++) {
                    fmodGUIDProperty.GetArrayElementAtIndex(i).intValue = (int)fmodguid[i];
                }

                fmodGUIDProperty.serializedObject.ApplyModifiedProperties();
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
                SerializedProperty fmodStringProperty = property.FindPropertyRelative("stringFMODRef");
                bool expanded = fmodStringProperty.isExpanded && !string.IsNullOrEmpty(fmodStringProperty.stringValue) && EventManager.EventFromPath(fmodStringProperty.stringValue) != null;
                float baseHeight = GUI.skin.textField.CalcSize(new GUIContent()).y;
                return baseHeight * (expanded ? 7 : 2); // 6 lines of info
            }
        }

    }
}
