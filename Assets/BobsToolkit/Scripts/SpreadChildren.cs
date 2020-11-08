using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpreadChildren : MonoBehaviour
{
    public Vector3 Extents = Vector3.one;
    public float Radius = 1f;
    public bool AccountForObjectScale = false;

    public Vector3 GetScaledExtents()
    {
        if (AccountForObjectScale)
        {
            return Vector3.Scale(Extents, transform.localScale);
        }
        else
        {
            return Extents;
        }
    }
    public Vector3 ScaledHalfExtents => GetScaledExtents() / 2f;

    public float GetScaledRadius()
    {
        return GetScaledRadius(AccountForObjectScale);
    }

    public float GetScaledRadius(bool scaled)
    {
        return scaled ? Radius: Radius / transform.localScale.magnitude;
    }

    public enum Distribution
    {
        CornerToCorner,
        RectangleXY,
        RectangleXZ,
        RectangleYZ,
        CircularXY,
        CircularXZ,
        CircularYZ
    }
    public Distribution distribution;

    public void Spread()
    {
        switch (distribution)
        {
            case Distribution.CornerToCorner:
                DistributeCornerToCorner();
                break;
            case Distribution.RectangleXY:
                DistributePerimeter(Vector3.right + Vector3.up);
                break;
            case Distribution.RectangleXZ:
                DistributePerimeter(Vector3.right + Vector3.forward);
                break;
            case Distribution.RectangleYZ:
                DistributePerimeter(Vector3.up + Vector3.forward);
                break;
            case Distribution.CircularXY:
                DistributeCircular(Vector3.right + Vector3.up);
                break;
            case Distribution.CircularXZ:
                DistributeCircular(Vector3.right + Vector3.forward);
                break;
            case Distribution.CircularYZ:
                DistributeCircular(Vector3.up + Vector3.forward);
                break;
        }


    }

    public void DistributeCornerToCorner()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            float perimeter = Extents.x + Extents.z;
            float progress = i / (float)(transform.childCount - 1);
            Debug.Log(progress);
            Vector3 newPos = new Vector3();

            newPos = Vector3.Lerp(-ScaledHalfExtents, ScaledHalfExtents, progress);
            transform.GetChild(i).localPosition = newPos;
        }
    }

    public void DistributePerimeter(Vector3 axes)
    {
        Vector3 scaledExtentsAxisConstrained = Vector3.Scale(ScaledHalfExtents, axes);

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform currentChild = transform.GetChild(i);
            float progress = i / (float)(transform.childCount);
            // Circular distribution coordinates
            // 0.00 = (0, 0)
            // 0.25 = (1, 0)
            // 0.50 = (1, 1)
            // 0.75 = (0, 1)

            float[] vs = { .25f, .5f, .75f, 1f };
            float partialProgress;
            Vector3 minPos;
            Vector3 maxPos;
            
            if (progress < vs[0])
            {
                partialProgress = Mathf.InverseLerp(0f, vs[0], progress);
                minPos = new Vector3(-scaledExtentsAxisConstrained.x, -scaledExtentsAxisConstrained.y, -scaledExtentsAxisConstrained.z);
                maxPos = new Vector3(scaledExtentsAxisConstrained.x, -scaledExtentsAxisConstrained.y, -scaledExtentsAxisConstrained.z);
            }
            else if (progress < vs[1])
            {
                partialProgress = Mathf.InverseLerp(vs[0], vs[1], progress);
                minPos = new Vector3(scaledExtentsAxisConstrained.x, -scaledExtentsAxisConstrained.y, -scaledExtentsAxisConstrained.z);
                maxPos = new Vector3(scaledExtentsAxisConstrained.x, scaledExtentsAxisConstrained.y, scaledExtentsAxisConstrained.z);
            }
            else if (progress < vs[2])
            {
                partialProgress = Mathf.InverseLerp(vs[1], vs[2], progress);
                minPos = new Vector3(scaledExtentsAxisConstrained.x, scaledExtentsAxisConstrained.y, scaledExtentsAxisConstrained.z);
                maxPos = new Vector3(-scaledExtentsAxisConstrained.x, scaledExtentsAxisConstrained.y, scaledExtentsAxisConstrained.z);
            }
            else
            {
                partialProgress = Mathf.InverseLerp(vs[2], vs[3], progress);
                minPos = new Vector3(-scaledExtentsAxisConstrained.x, scaledExtentsAxisConstrained.y, scaledExtentsAxisConstrained.z);
                maxPos = new Vector3(-scaledExtentsAxisConstrained.x, -scaledExtentsAxisConstrained.y, -scaledExtentsAxisConstrained.z);
            }
            Vector3 newPosition = Vector3.Lerp(minPos, maxPos, partialProgress);
            currentChild.localPosition = newPosition;
            //Debug.Log("Min pos: " + minPos + " - Max pos: " + maxPos + " - Progress: " + progress + " - Partial Progress: " + partialProgress + " - output position: " + newPosition);
        }
    }

    public void DistributeCircular(Vector3 axes)
    {
        float radius = GetScaledRadius();
        Debug.Log("Scaled radius: " + radius);
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform currentChild = transform.GetChild(i);
            float progress = i / (float)(transform.childCount);
            progress *= Mathf.PI * 2f;

            //Debug.Log("i = " + i + "--- Sine = " + Mathf.Sin(i));
            Vector3 newPos = new Vector3(Mathf.Sin(progress), Mathf.Cos(progress), Mathf.Cos(progress));
            newPos *= radius;
            //newPos /= Radius;
            newPos.Scale(axes);
            currentChild.localPosition = newPos;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Handles.matrix = transform.localToWorldMatrix;
        switch (distribution)
        {
            case Distribution.CornerToCorner:
            case Distribution.RectangleXY:
            case Distribution.RectangleXZ:
            case Distribution.RectangleYZ:
                Gizmos.DrawWireCube(Vector3.zero, GetScaledExtents());
                break;
            case Distribution.CircularXY:
                Handles.DrawWireDisc(Vector3.zero, Vector3.forward, GetScaledRadius());
                break;
            case Distribution.CircularXZ:
                Handles.DrawWireDisc(Vector3.zero, Vector3.up, GetScaledRadius());
                break;
        }
    }
#endif
}
