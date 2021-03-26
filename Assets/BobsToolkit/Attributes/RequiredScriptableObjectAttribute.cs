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
    internal int additionalLines = 0;
    internal bool viableProperty = true;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float space = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        return base.GetPropertyHeight(property, label) + space * additionalLines; ;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        RequiredScriptableObjectAttribute att = attribute as RequiredScriptableObjectAttribute;
        Type T = property.GetType();
        viableProperty = property.propertyType == SerializedPropertyType.ObjectReference;
        additionalLines = 0;

        if (!viableProperty)
        {
            EditorGUI.LabelField(position, label.text, "Field type is not an object reference.");
            return;
        }
        float space = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        position.height = space;
        EditorGUI.PropertyField(position, property, label);
        if (property.objectReferenceValue == null)
        {
            additionalLines += 1;
            position.y += space;
            EditorGUI.HelpBox(position, "This is a required object", MessageType.Warning);

            //additionalLines += 1;
            //position.y += space;
            //if (GUI.Button(position, "Add required scriptable object"))
            //{
            //    Debug.Log("Create and add new scriptable object file");
            //}

            //position.height += 2 * space;
            //position.yMax += 2 * space;
            return;
        }
    }
}
