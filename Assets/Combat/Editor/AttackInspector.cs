using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Combat;

[CanEditMultipleObjects]
[CustomEditor(typeof(Attack))]
public class AttackInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Attack targetScript = (Attack)target;

        switch (targetScript.effect)
        {
            case Attack.Effect.Health:
                targetScript.Damage = EditorGUILayout.IntField("Health Damage", targetScript.Damage);
                break;
            case Attack.Effect.Stamina:
                targetScript.StaminaReduction = EditorGUILayout.IntField("Stamina Reduction", targetScript.StaminaReduction);
                break;
            case Attack.Effect.MovementSpeed:
                targetScript.MovementSpeedReduction = EditorGUILayout.Slider(targetScript.MovementSpeedReduction, 0f, 1f);
                break;
        }
    }
}
