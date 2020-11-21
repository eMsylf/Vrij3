using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Torus))]
public class TorusEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Torus targetScript = (Torus)target;
        //base.OnInspectorGUI();
        //if (GUILayout.Button("Update torus"))
        //{
        //    Debug.Log("Update torus", this);
        //    targetScript.Recalculate(out Vector3[] verts, out int[] inds);
        //    targetScript.UpdateMesh(verts, inds);
        //}

        EditorGUI.BeginChangeCheck();
        targetScript.radius = EditorGUILayout.FloatField("Radius", targetScript.radius);
        targetScript.thickness= EditorGUILayout.FloatField("Thickness", targetScript.thickness);
        targetScript.segments = EditorGUILayout.IntField("Segments", targetScript.segments);
        targetScript.segmentDetail = EditorGUILayout.IntField("Segment Detail", targetScript.segmentDetail);
        if (EditorGUI.EndChangeCheck())
        {
            targetScript.Recalculate(out Vector3[] verts, out int[] inds);
            targetScript.UpdateMesh(verts, inds);
        }
    }
}
