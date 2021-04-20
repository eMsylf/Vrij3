using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Death))]
public class DeathInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Death targetScript = (Death)target;
        if (GUILayout.Button("Die"))
        {
            targetScript.Die();
        }
        if (GUILayout.Button("Resurrect"))
        {
            targetScript.Resurrect();
        }
        if (GUILayout.Button("Release Energy"))
        {
            targetScript.ReleaseEnergy();
        }
    }
}
