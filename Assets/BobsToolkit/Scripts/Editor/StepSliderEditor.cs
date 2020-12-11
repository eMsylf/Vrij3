using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BobJeltes
{
    [CustomEditor(typeof(StepSlider))]
    public class StepSliderEditor : Editor
    {
        private int newMax;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            StepSlider targetScript = (StepSlider)target;

            EditorGUI.BeginChangeCheck();
            newMax = Mathf.Max(1, EditorGUILayout.IntField("Slider max", targetScript.Max));
            if (EditorGUI.EndChangeCheck())
            {
                targetScript.UpdateMax(newMax);
            }


            EditorGUI.BeginChangeCheck();
            targetScript.Current = EditorGUILayout.IntSlider("Value", targetScript.Current, 0, targetScript.Max);
            if (EditorGUI.EndChangeCheck())
            {
                targetScript.UpdateCurrent();
            }
        }
    }
}