using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritePlaneDouble : MonoBehaviour
{
    public SpritePlane Front;
    public SpritePlane Back;

    [Header("Setup")]
    [Tooltip("Make a material out of the sprite and provide it here")]
    public Material material;
    public enum MeshType
    {
        Quad,
        Plane
    }
    public MeshType meshType;
    public Mesh Quad;
    public Mesh Plane;
    
    [Header("Settings")]
    [Min(0.00001f)]
    public float scale = 1f;
    public bool invertHorizontal;
    public bool invertVertical;

    private void OnValidate()
    {
        if (!PlanesSetCorrectly())
        {
            AddNewPlanes();
        }

        Front.UpdateScale(scale, (SpritePlane.Orientation)meshType, GetMesh(), material, invertHorizontal, invertVertical);
        Back.UpdateScale(scale, (SpritePlane.Orientation)meshType, GetMesh(), material, !invertHorizontal, invertVertical);
    }

    private bool PlanesSetCorrectly()
    {
        if (Front != null && Back != null)
        {
            return true;
        }
        return false;
    }

    private void AddNewPlanes()
    {
        if (Front == null)
        {
            GameObject newFront = new GameObject("Front");
            newFront.transform.SetParent(transform);
            Vector3 pos = new Vector3(0f, 0f, -.001f);
            newFront.transform.localPosition = pos;
            Vector3 rot = new Vector3(0f, 180f, 0f);
            newFront.transform.localRotation = Quaternion.Euler(rot);
            newFront.transform.localScale = Vector3.one;
            Front = newFront.AddComponent<SpritePlane>();
        }
        if (Back == null)
        {
            GameObject newBack = new GameObject("Back");
            newBack.transform.SetParent(transform);
            Vector3 pos = new Vector3(0f, 0f, .001f);
            newBack.transform.localPosition = pos;
            newBack.transform.localRotation = Quaternion.identity;
            newBack.transform.localScale = Vector3.one;
            Back = newBack.AddComponent<SpritePlane>();
        }
    }

    public Mesh GetMesh()
    {
        switch (meshType)
        {
            default:
            case MeshType.Quad:
                if (Quad == null) Debug.LogError("Quad mesh not set");
                else return Quad;
                break;
            case MeshType.Plane:
                if (Plane == null) Debug.LogError("Plane mesh not set");
                else return Plane;
                break;
        }
        return null;
    }
}
