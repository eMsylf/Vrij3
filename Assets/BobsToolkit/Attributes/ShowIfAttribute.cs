using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BobJeltes.Attributes
{
    public class ShowIfAttribute : PropertyAttribute
    {
        private string valueName;
        private bool boolValue;

        private bool invert;
        private int[] enumIndices;

        internal string ValueName { get => valueName; private set => valueName = value; }
        internal bool BoolValue { get => boolValue; private set => boolValue = value; }
        internal int[] EnumIndices { get => enumIndices; private set => enumIndices = value; }
        internal bool Invert { get => invert; private set => invert = value; }

        /// <summary>
        /// Only shows the parameter that this is used on, if the chosen value (with the value name) matches the chosen value. NOTE: Does not work in combination with other attributes.
        /// </summary>
        /// <param name="boolValueName">The name of the reference value that determines whether the property is shown in the inspector or not</param>
        /// <param name="boolValue">The bool value at which you want the parameter to be shown</param>
        public ShowIfAttribute(string boolValueName, bool boolValue = true)
        {
            ValueName = boolValueName;
            this.BoolValue = boolValue;
        }
        /// <summary>
        /// Only shows the parameter that this is used on, if the chosen parameter matches the chosen value. NOTE: Does not work in combination with other attributes.
        /// </summary>
        /// <param name="enumValueName">The name of the enum value.</param>
        /// <param name="enumIndices">The index of the enum that you want the parameter to be shown at.</param>
        public ShowIfAttribute(string enumValueName, bool invert = false, params int[] enumIndices)
        {
            ValueName = enumValueName;
            this.Invert = invert;
            this.EnumIndices = enumIndices;
        }
    }

    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfPropertyDrawer : PropertyDrawer
    {
        ShowIfAttribute att;
        SerializedProperty condition;
        bool show;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (att == null || condition == null)
                return base.GetPropertyHeight(property, label);

            if (show)
            {
                return base.GetPropertyHeight(property, label);
            }
            else
            {
                return -2;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty conditional = property.serializedObject.FindProperty((attribute as ShowIfAttribute).ValueName);
            condition = conditional;
            if (conditional == null)
            {
                Debug.LogError("Property name entered in ShowIf attribute does not match an existing property");
            }
            att = attribute as ShowIfAttribute;

            switch (conditional.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    show = conditional.boolValue == att.BoolValue;
                    break;
                case SerializedPropertyType.Enum:
                    show = MatchesEnumValue(conditional);
                    break;
                default:
                    Debug.LogError("Referenced property '" + att.ValueName + "' is not a boolean or enum");
                    break;
            }

            if (show)
            {
                EditorGUI.PropertyField(position, property, true);
            }
        }

        public bool MatchesEnumValue(SerializedProperty enumValue)
        {
            for (int i = 0; i < att.EnumIndices.Length; i++)
            {
                if (enumValue.enumValueIndex == att.EnumIndices[i])
                    return att.Invert ? false : true;
            }
            return false;
        }
    }
}