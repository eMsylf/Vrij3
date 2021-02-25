using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering.PostProcessing;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SpritePlane : MonoBehaviour
{
    public enum Orientation
    {
        XY,
        XZ
    }
    public Orientation orientation;
    [Min(0.00001f)]
    public float scale = 1;
    public bool invertHorizontal;
    public bool invertVertical;

    private MeshFilter meshFilter;
    private MeshFilter GetMeshFilter()
    {
        if (meshFilter == null)
        {
            meshFilter = GetComponent<MeshFilter>();
        }
        return meshFilter;
    }
    private MeshRenderer meshRenderer;
    private MeshRenderer GetMeshRenderer()
    {
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        return meshRenderer;
    }

    private void OnValidate()
    {
        SetScale();
    }

    public void UpdateScale(float scale, Orientation orientation, Mesh mesh, Material material, bool flipHorizontal, bool flipVertical)
    {
        SetMaterial(material);
        SetMesh(mesh);
        this.scale = scale;
        this.orientation = orientation;
        this.invertHorizontal = flipHorizontal;
        this.invertVertical = flipVertical;
        SetScale();
    }

    public void SetScale()
    {
        Texture texture = GetMeshRenderer().sharedMaterial.GetTexture("_MainTex");

        if (texture == null)
        {
            Debug.Log("No texture found in the provided material");
            return;
        }

        Vector3 newScale = Vector3.one;
        switch (orientation)
        {
            case Orientation.XY:
                newScale.x = texture.width * (invertHorizontal ? -scale : scale);
                newScale.y = texture.height * (invertVertical ? -scale : scale);
                break;
            case Orientation.XZ:
                newScale.x = texture.width * (invertHorizontal ? -scale : scale);
                newScale.z = texture.height * (invertVertical ? -scale : scale);
                break;
            default:
                break;
        }

        transform.localScale = newScale;
    }

    internal void SetMaterial(Material material)
    {
        if (GetMeshRenderer().sharedMaterial != material)
            GetMeshRenderer().sharedMaterial = material;
    }

    internal void SetMesh(Mesh mesh)
    {
        if (GetMeshFilter().sharedMesh != mesh)
            GetMeshFilter().sharedMesh = mesh;
    }
}
