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
        Texture texture = GetComponent<MeshRenderer>().sharedMaterial.GetTexture("_MainTex");

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

        //if (sprite == null)
        //    return;
        //switch (orientation)
        //{
        //    case Orientation.XY:
        //        newScale.x = sprite.rect.width * (invertHorizontal ? -scale : scale);
        //        newScale.y = sprite.rect.height * (invertVertical ? -scale : scale);
        //        break;
        //    case Orientation.XZ:
        //        newScale.x = sprite.rect.width * (invertHorizontal ? -scale : scale);
        //        newScale.z = sprite.rect.height * (invertVertical ? -scale : scale);
        //        break;
        //    default:
        //        break;
        //}

        transform.localScale = newScale;
    }

    internal void SetMaterial(Material material)
    {
        GetComponent<MeshRenderer>().sharedMaterial = material;
    }

    internal void SetMesh(Mesh mesh)
    {
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}
