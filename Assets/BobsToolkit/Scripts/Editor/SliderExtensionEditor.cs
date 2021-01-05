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

            //Undo.RecordObject(target, "Set slider max");
            EditorGUI.BeginChangeCheck();
            newMax = Mathf.Max(1, EditorGUILayout.FloatField("Slider max", targetScript.maxValue));
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                targetScript.SetMax(newMax);
            }


            //Undo.RecordObject(target, "Set slider value");
            EditorGUI.BeginChangeCheck();
            newCurrent = EditorGUILayout.Slider("Value", targetScript.m_value, 0, targetScript.maxValue);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
                targetScript.SetValue(newCurrent);
            }
            base.OnInspectorGUI();
        }
    }
}