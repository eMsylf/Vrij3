using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpreadChildren))]
public class SpreadChildrenInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        SpreadChildren targetScript = (SpreadChildren)target;
        if (GUILayout.Button("Spread Children"))
        {
            Debug.Log("Spread Children", target);
            EditorUtility.SetDirty(target);
            targetScript.Spread();
        }
    }
}
