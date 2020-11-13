using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DistributeChildren))]
public class DistributeChildrenInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DistributeChildren targetScript = (DistributeChildren)target;
        if (GUILayout.Button("Distribute Children"))
        {
            Debug.Log("Distribute Children", target);
            EditorUtility.SetDirty(target);
            targetScript.Spread();
        }
    }
}
