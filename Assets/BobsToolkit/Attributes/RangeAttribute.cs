﻿using System;
using UnityEngine;
using UnityEditor;

namespace BobJeltes.Attributes
{
    public class RangeAttribute : PropertyAttribute
    {
        public float min;
        public float max;

        public RangeAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }

    [CustomPropertyDrawer(typeof(RangeAttribute))]
    public class RangeDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // First get the attribute since it contains the range for the slider
            RangeAttribute range = attribute as RangeAttribute;

            // Now draw the property as a Slider or an IntSlider based on whether it's a float or integer.
            if (property.propertyType == SerializedPropertyType.Float)
                EditorGUI.Slider(position, property, range.min, range.max, label);
            else if (property.propertyType == SerializedPropertyType.Integer)
                EditorGUI.IntSlider(position, property, Convert.ToInt32(range.min), Convert.ToInt32(range.max), label);
            else
                EditorGUI.LabelField(position, label.text, "Use Range with float or int.");
        }
    }
}