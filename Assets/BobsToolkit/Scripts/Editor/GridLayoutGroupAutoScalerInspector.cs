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

        //if (GUILayout.Button("Calculate remaining pixels"))
        //{
        //    targetScript.CalculateCellSize();
        //}

        EditorGUILayout.HelpBox("This component is designed to work for GridLayoutGroups that " +
            "\nuse 1 row or column", 
            MessageType.Info);

        Undo.RecordObject(targetScript.GetComponent<GridLayoutGroup>(), "Fit into padding on selected axis");

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
