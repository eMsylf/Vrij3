using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DayNightCycle))]
public class DayNightCycleEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DayNightCycle targetScript = (DayNightCycle)target;
        // Updates / cycle (updates nodig voor een heel rondje)
        float durationS = ((360f / targetScript.Speed));
        float durationM = durationS / 60f;
        float durationH = durationM / 60f;
        float durationD = durationH / 24f;
        EditorGUILayout.LabelField("Day duration (seconds)", durationS.ToString());
        EditorGUILayout.LabelField("Day duration (minutes)", durationM.ToString());
        EditorGUILayout.LabelField("Day duration (hours)", durationH.ToString());
        EditorGUILayout.LabelField("Day duration (days)", durationD.ToString());
    }
}
