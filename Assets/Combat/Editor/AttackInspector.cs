using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Gyrus.Combat;

[CanEditMultipleObjects]
[CustomEditor(typeof(Damager))]
public class AttackInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Damager targetScript = (Damager)target;

        switch (targetScript.effect)
        {
            case Damager.Effect.Health:
                targetScript.Damage = EditorGUILayout.IntField("Health Damage", targetScript.Damage);
                break;
            case Damager.Effect.Stamina:
                targetScript.StaminaReduction = EditorGUILayout.IntField("Stamina Reduction", targetScript.StaminaReduction);
                break;
            case Damager.Effect.MovementSpeed:
                targetScript.MovementSpeedReduction = EditorGUILayout.Slider(targetScript.MovementSpeedReduction, 0f, 1f);
                break;
        }
    }
}
