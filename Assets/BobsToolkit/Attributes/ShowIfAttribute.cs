using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BobJeltes.Attributes
{
    public class ShowIfAttribute : PropertyAttribute
    {
        public string boolValueName;
        public enum Type { Boolean, Enum }
        public Type type;
        public int index;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boolValueName">Reference value that determines whether this property is shown in the inspector or not</param>
        /// <param name="type">Choose between a boolean or enum</param>
        /// <param name="index">Show if the enum index matches this index</param>
        public ShowIfAttribute(string boolValueName, Type type = Type.Boolean, int index = 0)
        {
            this.boolValueName = boolValueName;
            this.type = type;
            this.index = index;
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

            return show?base.GetPropertyHeight(property,label):0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty conditional = property.serializedObject.FindProperty((attribute as ShowIfAttribute).boolValueName);
            att = attribute as ShowIfAttribute;
            condition = conditional;
            if (conditional == null)
            {
                Debug.LogError("Property name entered in ShowIf attribute does not match an existing property");
            }

            switch (att.type)
            {
                case ShowIfAttribute.Type.Boolean:
                    if (conditional.propertyType != SerializedPropertyType.Boolean)
                    {
                        Debug.LogError("Referenced property by ShowIf attribute is not a boolean");
                        return;
                    }
                    show = conditional.boolValue;
                    break;
                case ShowIfAttribute.Type.Enum:
                    if (conditional.propertyType != SerializedPropertyType.Enum)
                    {
                        Debug.LogError("Referenced property by ShowIf attribute is not an enum");
                        return;
                    }
                    show = conditional.enumValueIndex == att.index;
                    break;
                default:
                    break;
            }

            if (show)
            {
                EditorGUI.PropertyField(position, property);
            }
        }
    }
}