using UnityEditor;
using UnityEngine;

public class MeshExporter : MonoBehaviour
{
    [MenuItem("GameObject/Export Mesh")]
    public static void ExportMesh()
    {
        System.Type type = Selection.activeObject.GetType();

        GameObject selectedGameObject = Selection.activeGameObject;
        MeshFilter mf = selectedGameObject.GetComponent<MeshFilter>();

        if (mf == null)
        {
            Debug.LogError("Selected object has no mesh filter.", selectedGameObject);
            return;
        }
        if (mf.sharedMesh == null)
        {
            Debug.LogError("Selected object's mesh filter has no mesh.", mf);
        }

        Mesh mesh = Instantiate(mf.sharedMesh);

        string meshFolder = "Assets/Meshes";
        string meshName = "Mesh export";
        //if (string.IsNullOrEmpty(meshName))
        //{
        //    meshName = "New mesh";
        //}

        // If the folder doesn't exist yet, create a folder
        if (!AssetDatabase.IsValidFolder(meshFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Meshes");
            return;
        }

        // If an asset exists at the same place with the same name
        string assetPath = meshFolder + "/" + meshName + ".mesh";
        assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
        
        AssetDatabase.CreateAsset(mesh, assetPath);

        Debug.Log("Saved new mesh to " + AssetDatabase.GetAssetPath(mesh));
        if (EditorUtility.DisplayDialog("Keep editing mesh?", "Would you like to keep editing the mesh after exporting? This will change the saved mesh file when you edit it on the object.", "Yes", "No, create new instance"))
        {
            mf.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
        }
        else
        {
            mf.sharedMesh = new Mesh();
            mf.sharedMesh.name = "New mesh";
        }

        //if (string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(assetPath)))
        //{
        //    AssetDatabase.CreateAsset(mesh, assetPath);
        //    Debug.Log(AssetDatabase.GetAssetPath(mesh));
        //}
        //else
        //{

        //    if (EditorUtility.DisplayDialog("Overwrite existing mesh?", "A torus mesh already exists at path " + assetPath + ". Would you like to overwrite it?", "Overwrite", "Cancel"))
        //    {
        //        AssetDatabase.DeleteAsset(assetPath);
        //        AssetDatabase.CreateAsset(mesh, assetPath);
        //        Debug.Log(AssetDatabase.GetAssetPath(mesh));
        //    }
        //}
    }

    //private Mesh DuplicateMesh(Mesh mesh)
    //{
    //    Mesh newMesh = new Mesh();
    //    newMesh.vertices = mesh.vertices;
    //    newMesh.triangles = mesh.triangles;
    //    newMesh.uv = mesh.uv;
    //    newMesh.uv2 = mesh.uv2;
    //    //Vertices
    //    //Triangles
    //    //UV, uv2
    //    //normal
    //    //tangent
    //    //colors
    //}
}
