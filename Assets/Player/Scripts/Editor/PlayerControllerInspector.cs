using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Apply charge zone colors"))
        {
            PlayerController targetScript = (PlayerController)target;

            EditorUtility.SetDirty(target);
            targetScript.attacking.ApplyChargeZoneColors();
        }
    }
}
