using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LookAtTargetGroup))]
public class LookAtTargetGroupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        LookAtTargetGroup targetSript = (LookAtTargetGroup)target;
        if (GUILayout.Button("Look At Target"))
        {
            targetSript.DoLookAt();
        }
    }
}
