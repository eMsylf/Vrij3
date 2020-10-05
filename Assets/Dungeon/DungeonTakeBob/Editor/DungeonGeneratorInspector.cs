using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DungeonGenerator))]
public class DungeonGeneratorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DungeonGenerator targetScript = (DungeonGenerator)target;
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Dungeon"))
        {
            if (targetScript.transform.childCount > 0 && !EditorUtility.DisplayDialog("Overwrite current dungeon", "All of the dungeon's children will be destroyed. Continue?", "Yes", "Cancel"))
            {
                Debug.Log("Dungeon generation cancelled.");
            }
            else
            {
                Debug.Log("Generate dungeon");
                targetScript.Generate();
            }
        }

        if (GUILayout.Button("Destroy Dungeon"))
        {
            if (EditorUtility.DisplayDialog("Destroy current dungeon", "All of the dungeon's children will be destroyed. Continue?", "Yes", "Cancel"))
            {
                Debug.Log("Destroy dungeon");
                targetScript.DestroyDungeon();
            }
        }
    }
}
