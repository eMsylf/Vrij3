using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BobJeltes.Attributes
{
    public class ButtonAttribute : PropertyAttribute
    {
        public string name;

        public ButtonAttribute(string name)
        {
            this.name = name;
        }
    }

    [CustomPropertyDrawer(typeof(ButtonAttribute))]
    public class ButtonDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ButtonAttribute ah = attribute as ButtonAttribute;
            base.OnGUI(position, property, label);
            if (GUI.Button(position, ah.name))
            {
                Debug.Log("HAAAAAAAAAAAAA");
                SerializedProperty prop = property.FindPropertyRelative(ah.name);
                
                if (prop.propertyType == SerializedPropertyType.ObjectReference)
                {

                }
            }
        }
    }
}
