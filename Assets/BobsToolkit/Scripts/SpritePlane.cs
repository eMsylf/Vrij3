using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering.PostProcessing;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SpritePlane : MonoBehaviour
{
    public Sprite sprite;
    [Min(0.00001f)]
    public float scale = 1;
    public bool invertX;
    public bool invertY;

    private void OnValidate()
    {
        SetScale();
    }

    public void SetScale()
    {
        if (sprite == null)
            return;
        Vector3 newScale = Vector3.one;
        newScale.x = sprite.rect.width * (invertX?-scale:scale);
        newScale.y = sprite.rect.height * (invertY?-scale:scale);

        transform.localScale = newScale;
    }
}
