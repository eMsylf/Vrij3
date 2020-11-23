using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Torus))]
public class TorusEditor : Editor
{
    public string ExportName = "Torus";

    public override void OnInspectorGUI()
    {
        Torus targetScript = (Torus)target;

        //if (targetScript.GetComponent<MeshRenderer>().sharedMaterial == null)
        //{
        //    targetScript.UpdateTorus();
        //}

        EditorGUI.BeginChangeCheck();
        targetScript.radius = Mathf.Max(.001f, EditorGUILayout.FloatField("Radius", targetScript.radius));
        targetScript.thickness = Mathf.Max(.001f, EditorGUILayout.FloatField("Thickness", targetScript.thickness));
        targetScript.segments = Mathf.Max(3, EditorGUILayout.IntField("Segments", targetScript.segments));
        targetScript.segmentDetail = Mathf.Max(3, EditorGUILayout.IntField("Segment Detail", targetScript.segmentDetail));
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
            targetScript.UpdateTorus();
        }
        ExportName = EditorGUILayout.TextField("Export name", ExportName);

        if (GUILayout.Button("Export mesh"))
        {
            targetScript.UpdateTorus();
            Mesh mesh = targetScript.Mf.sharedMesh;
            string folderPath = "Assets/Meshes";
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                folderPath = AssetDatabase.CreateFolder("Assets", "Meshes");
            }

            string assetPath = folderPath + "/" + ExportName + ".mesh";

            if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(assetPath))) 
            {
            }
            else
            {
                Debug.Log("An asset already exists at path " + assetPath);
                if (EditorUtility.DisplayDialog("Overwrite existing torus mesh?", "A torus mesh already exists at path " + assetPath + ". Would you like to overwrite it?", "Overwrite", "Cancel"))
                {
                    AssetDatabase.DeleteAsset(assetPath);
                    AssetDatabase.CreateAsset(mesh, assetPath);
                    Debug.Log(AssetDatabase.GetAssetPath(mesh));
                }
            }

            
        }
    }
}
