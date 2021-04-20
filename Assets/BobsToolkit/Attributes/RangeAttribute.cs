using System;
using UnityEngine;
using UnityEditor;

namespace BobJeltes.Attributes.Experimental
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
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                default:
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.Float:
                    return base.GetPropertyHeight(property, label);
                case SerializedPropertyType.Vector2:
                    return base.GetPropertyHeight(property, label) * 2;
            }
        }

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // First get the attribute since it contains the range for the slider
            RangeAttribute range = attribute as RangeAttribute;

            // Now draw the property as a Slider or an IntSlider based on whether it's a float or integer.
            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                    EditorGUI.Slider(position, property, range.min, range.max, label);
                    break;
                case SerializedPropertyType.Integer:
                    EditorGUI.IntSlider(position, property, Convert.ToInt32(range.min), Convert.ToInt32(range.max), label);
                    break;
                case SerializedPropertyType.Vector2:
                    Vector2 storedValue = property.vector2Value;

                    position.height = EditorGUIUtility.singleLineHeight;
                    // TODO: Move this to MinMaxSlider attribute
                    EditorGUI.MinMaxSlider(position, label.text, ref storedValue.x, ref storedValue.y, range.min, range.max);
                    property.vector2Value = storedValue;
                    GUIContent[] labels = {
                        new GUIContent(),
                        new GUIContent(),
                        new GUIContent(),
                        new GUIContent(),
                    };
                    // TODO: a vertical slider for the y value
                    float[] values = { range.min, storedValue.x, storedValue.y, range.max };
                    position.y += EditorGUIUtility.singleLineHeight;
                    //position.y += EditorGUIUtility.singleLineHeight;
                    EditorGUI.MultiFloatField(position, labels, values);
                    break;
                default:
                    EditorGUI.LabelField(position, label.text, "Use Range with float or int.");
                    break;
            }
        }
    }
}