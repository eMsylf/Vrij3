using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BobJeltes
{
    [CustomEditor(typeof(SliderExtension))]
    public class SliderExtensionEditor : Editor
    {
        private float newMax;
        private float newCurrent;
        public override void OnInspectorGUI()
        {
            SliderExtension targetScript = (SliderExtension)target;

            EditorGUI.BeginChangeCheck();
            newMax = Mathf.Max(1, EditorGUILayout.FloatField("Slider max", targetScript.maxValue));
            if (EditorGUI.EndChangeCheck())
            {
                targetScript.SetMax(newMax);
            }


            EditorGUI.BeginChangeCheck();
            newCurrent = EditorGUILayout.Slider("Value", targetScript.value, 0, targetScript.maxValue);
            if (EditorGUI.EndChangeCheck())
            {
                targetScript.SetValue(newCurrent);
            }
            base.OnInspectorGUI();
        }
    }
}