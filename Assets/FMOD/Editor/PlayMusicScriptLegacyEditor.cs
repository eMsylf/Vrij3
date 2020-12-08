using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayMusicScriptLegacy))]
public class PlayMusicScriptLegacyEditor : Editor
{
    public float input = 0;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Label("Editor");
        input = EditorGUILayout.FloatField("Input level", input);
        PlayMusicScriptLegacy targetScript = (PlayMusicScriptLegacy)target;
        if (GUILayout.Button("Set Anxiety"))
        {
            targetScript.SetAnxiety(input);
        }
        if (GUILayout.Button("Set Battle"))
        {
            targetScript.SetBattle(input);
        }
        if (GUILayout.Button("Set Curiosity"))
        {
            targetScript.SetCuriousity(input);
        }
    }
}
