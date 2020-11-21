/**
 * Based on a script by thieberson (http://forum.unity3d.com/threads/torus-in-unity.8487/) (in Torus.zip, originally named Torus.cs),
 * which was based on a script by Steffen ("Primitives.cs" from $primitives_966_104.zip).
 * 
 * Editted by Michael Zoller on December 6, 2015.
 * 
 * Usage Notes:
 * If the color property is preventing you from changing the color in a different Script's start, give this script Execution Order priority.
 * 
 * Paraphrase of original usage notes:
 * This version is improved from Steffen's original to allow the manipulation of the ring outside the script (ex. in the Unity editor while testing).
 * The script can be attached to any GameObject (Main), although an Emtpy one is best.
 * When the script starts, it creates a sibling GameObject to be the ring (meshRing).
 * The user can change the segmentRadius, tubeRadius and numTubes of the meshRing through the
 * transform.scale.x, transform.scale.y and transform.scale.z, respectively, of Main.
 * The position, rotation and color of the meshRing are copied from Main.
 * 
 * Outside the script, the transform.scale of Main can be accessed by: GameObject.Find(name_of_the_Main_Game_Object)
 * 
 * Edit: By Bob Jeltes
 * 
 */
using UnityEngine;
using UnityEditor;

public class TorusCreator : MonoBehaviour {

	#region Manipulate Torus
    // If you don't need to see what values they were assigned, these can be made non-public
	public float segmentRadius = 1f;
	public float tubeRadius = 0.1f;
	public int numSegments = 32;
	public int numTubes = 12;

    [MenuItem("GameObject/3D Object/Torus", false, 10)]
    public static void CreateTorus()
    {
        //new Torus(new GameObject("New torus", typeof(MeshFilter), typeof(MeshRenderer)), 1f, .1f, 32, 12);
        GameObject torus = new GameObject("New torus", typeof(Torus));
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("High-Definition Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Lit");
        Material mat = new Material(shader);
        torus.GetComponent<MeshRenderer>().sharedMaterial = mat;
        //torus.GetComponent<MeshRenderer>().sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Lit");
    }

    public void torus(GameObject torusMesh, float segmentRadius, float tubeRadius, int numSegments, int numTubes)
    {
        
    }
	#endregion

    public string torusGameObjectName = "TorusMesh";
    public Color color = Color.blue;
	GameObject torusMesh;
	Vector3 oldScale = Vector3.zero;
    Color oldColor = Color.clear;

	void Start () {
		torusMesh = new GameObject(torusGameObjectName);
        torusMesh.transform.parent = this.transform.parent;
		torusMesh.AddComponent<MeshFilter>();
		torusMesh.AddComponent<MeshRenderer>();
        Update(); // to allow other Script's Start() methods to change the color
	}
	
	void Update () {
		if (oldScale != transform.localScale) { // Chech if the parameters changed to improve performance
			segmentRadius = transform.localScale.x;
			tubeRadius = transform.localScale.y;
            numSegments = Mathf.RoundToInt(32 * (1 + segmentRadius / 10.0f));
			numTubes = Mathf.RoundToInt(transform.localScale.z);
			torus (torusMesh, segmentRadius, tubeRadius, numSegments, numTubes);
			oldScale = transform.localScale;
		}
        if (oldColor != color)
        {
            torusMesh.GetComponent<Renderer>().material.color = color;
            oldColor = color;
        }
		torusMesh.transform.position = transform.position;
		torusMesh.transform.rotation = transform.rotation;
	}

    public void UpdateTorus()
    {

    }
}
