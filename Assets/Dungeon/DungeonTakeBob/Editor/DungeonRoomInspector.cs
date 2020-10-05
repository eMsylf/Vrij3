using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DungeonRoom))]
public class DungeonRoomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DungeonRoom targetScript = (DungeonRoom)target;
        GUILayout.BeginHorizontal();
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Label("East");
                GUILayout.Label("South");
                GUILayout.Label("West");
                GUILayout.Label("East");

            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            {
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Toggle door"))
                    {
                        targetScript.North.ToggleDoor();
                    }
                    if (GUILayout.Button("Toggle opening"))
                    {
                        targetScript.North.ToggleOpening();
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Toggle door"))
                    {
                        targetScript.South.ToggleDoor();
                    }
                    if (GUILayout.Button("Toggle opening"))
                    {
                        targetScript.South.ToggleOpening();
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Toggle door"))
                    {
                        targetScript.West.ToggleDoor();
                    }
                    if (GUILayout.Button("Toggle opening"))
                    {
                        targetScript.West.ToggleOpening();
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Toggle door"))
                    {
                        targetScript.East.ToggleDoor();
                    }
                    if (GUILayout.Button("Toggle opening"))
                    {
                        targetScript.East.ToggleOpening();
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }GUILayout.EndHorizontal();
    }
}
