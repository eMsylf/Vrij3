using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CanEditMultipleObjects]
[CustomEditor(typeof(GridLayoutGroupAutoScaler))]
public class GridLayoutGroupAutoScalerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        GridLayoutGroupAutoScaler targetScript = (GridLayoutGroupAutoScaler)target;
        GridLayoutGroupAutoScaler[] targetScripts = new GridLayoutGroupAutoScaler[targets.Length];

        base.OnInspectorGUI();
        for (int i = 0; i < targets.Length; i++)
        {
            targetScripts[i] = (GridLayoutGroupAutoScaler)targets[i];
        }


        if (GUILayout.Button("Fit into padding"))
        {
            for (int i = 0; i < targetScripts.Length; i++)
            {
                EditorUtility.SetDirty(targetScripts[i]);
                targetScripts[i].AutoScale();

            }
        }

    }
}
