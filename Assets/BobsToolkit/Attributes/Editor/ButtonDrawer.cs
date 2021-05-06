using UnityEngine;
using UnityEditor;

namespace BobJeltes.Attributes
{
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
