using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RequiredScriptableObjectAttribute : PropertyAttribute
{

}

[CustomPropertyDrawer(typeof(RequiredScriptableObjectAttribute))]
public class RequiredScriptableObjectDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.objectReferenceValue == null)
        {
            float newHeight = base.GetPropertyHeight(property, label);

            float space = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            newHeight += space*2;
            return newHeight;

        }
        return base.GetPropertyHeight(property, label);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        RequiredScriptableObjectAttribute att = attribute as RequiredScriptableObjectAttribute;
        Type T = property.GetType();
        if (property.propertyType != SerializedPropertyType.ObjectReference)
        {
            EditorGUI.LabelField(position, "Field type is not object reference.");
            return;
        }
        float space = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        position.height = space;

        EditorGUI.PropertyField(position, property, label);
        if (property.objectReferenceValue == null)
        {
            position.y += space;
            EditorGUI.HelpBox(position, "This is a required scriptable object", MessageType.Warning);

            position.y += space;
            if (GUI.Button(position, "Add required scriptable object"))
            {
                Debug.Log("Create and add new scriptable object file");
            }

            //position.height += 2 * space;
            //position.yMax += 2 * space;
            return;
        }
    }
}
