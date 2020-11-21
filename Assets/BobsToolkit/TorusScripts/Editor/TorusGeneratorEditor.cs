using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TorusCreator))]
public class TorusGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TorusCreator targetScript = (TorusCreator)target;
        base.OnInspectorGUI();
        if (GUILayout.Button("Create torus"))
        {
            Debug.Log("Create torus", this);
            //targetScript.CreateTorus();
        }
    }
}
