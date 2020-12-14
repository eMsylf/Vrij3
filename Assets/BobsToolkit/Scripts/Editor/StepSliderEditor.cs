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
        private int newCurrent;
        public override void OnInspectorGUI()
        {
            StepSlider targetScript = (StepSlider)target;

            EditorGUI.BeginChangeCheck();
            newMax = Mathf.Max(1, EditorGUILayout.IntField("Slider max", targetScript.maxValue));
            if (EditorGUI.EndChangeCheck())
            {
                targetScript.SetMax(newMax);
            }


            EditorGUI.BeginChangeCheck();
            newCurrent = EditorGUILayout.IntSlider("Value", targetScript.value, 0, targetScript.maxValue);
            if (EditorGUI.EndChangeCheck())
            {
                targetScript.SetCurrent(newCurrent);
            }
            base.OnInspectorGUI();
        }
    }
}