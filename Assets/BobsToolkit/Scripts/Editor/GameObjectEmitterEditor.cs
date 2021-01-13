using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BobJeltes;

[CustomEditor(typeof(GameObjectEmitter))]
public class GameObjectEmitterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var targetScript = (GameObjectEmitter)target;
        if (targetScript.useObjectPool)
        {
            if (targetScript.objectPool == null && (targetScript.GetComponent<ObjectPool>() == null))
            {
                if (GUILayout.Button("Add objectpool"))
                {
                    EditorUtility.SetDirty(targetScript);
                    targetScript.objectPool = targetScript.gameObject.AddComponent<ObjectPool>();
                    targetScript.objectPool.prefab = targetScript.prefab;
                }
            }
        }
        
    }
}
