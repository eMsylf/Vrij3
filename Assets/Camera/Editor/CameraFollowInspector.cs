using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraFollow))]
public class CameraFollowInspector : Editor
{
    public override void OnInspectorGUI()
    {
        CameraFollow targetScript = (CameraFollow)target;
        base.OnInspectorGUI();

        if (GUILayout.Button("Inherit position offset from transform"))
        {
            targetScript.OverrideOffsets();
        }

        if (targetScript.LivePreview)
        {
            targetScript.SetPositionAndLook();
        }
    }
}
