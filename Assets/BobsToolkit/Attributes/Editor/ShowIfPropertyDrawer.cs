using UnityEngine;
using UnityEditor;

namespace BobJeltes.Attributes
{
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
                    return true;
            }
            return false;
        }
    }
}