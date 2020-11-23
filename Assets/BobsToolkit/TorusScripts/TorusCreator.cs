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
 * Edit by Bob Jeltes https://github.com/emsylf on November 22, 2020: 
 * This script used to do everything torus-related. 
 * I converted it to a creator script. 
 * Torus.cs is now its own class governing everything that has to do with the instance of a torus. 
 * TorusEditor.cs creates a custom inspector that allows you to see the effect of the changing variables in real-time.
 * This script was converted to an editor-script by Bob Jeltes https://github.com/emsylf
 */
using UnityEngine;
using UnityEditor;

public class TorusCreator : MonoBehaviour {
    [MenuItem("GameObject/3D Object/Torus", false, 10)]
    public static void CreateTorus()
    {
        GameObject torus = new GameObject("Torus", typeof(Torus));
        Material mat = getMaterial();
        torus.GetComponent<MeshRenderer>().sharedMaterial = mat;
        torus.GetComponent<Torus>().UpdateTorus();

        Selection.activeObject = torus;
    }

    public static Material getMaterial()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("High-Definition Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Lit");
        Material mat = new Material(shader);
        return mat;
    }
}
