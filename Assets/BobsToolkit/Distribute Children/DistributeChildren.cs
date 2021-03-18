using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DistributeChildren : MonoBehaviour
{
    public enum Distribution
    {
        CornerToCorner,
        RectangleXY,
        RectangleXZ,
        //RectangleYZ,
        CircularXY,
        CircularXZ,
        //CircularYZ
    }
    public Distribution distribution;
    public Vector3 Box = Vector3.one;
    public float Radius = 1f;
    public bool OverwriteChildrenRotation = false;

    public Vector3 HalfExtents => Box / 2f;

    private void OnValidate() => Spread();

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
            //case Distribution.RectangleYZ:
            //    DistributePerimeter(Vector3.up + Vector3.forward);
            //    break;
            case Distribution.CircularXY:
                DistributeCircular(Vector3.right + Vector3.up);
                break;
            case Distribution.CircularXZ:
                DistributeCircular(Vector3.right + Vector3.forward);
                break;
            //case Distribution.CircularYZ:
            //    DistributeCircular(Vector3.up + Vector3.forward);
            //    break;
        }
    }

    public void DistributeCornerToCorner()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            float progress = i / (float)(transform.childCount - 1);
            Vector3 newPos = Vector3.Lerp(-HalfExtents, HalfExtents, progress);
            transform.GetChild(i).localPosition = newPos;
        }
    }

    public void DistributePerimeter(Vector3 axes)
    {
        Vector3 scaledExtentsAxisConstrained = Vector3.Scale(HalfExtents, axes);
        float[] corners = { .25f, .5f, .75f, 1f };
        // Circular distribution coordinates
        // 0.00 = (0, 0)
        // 0.25 = (1, 0)
        // 0.50 = (1, 1)
        // 0.75 = (0, 1)

        float partialProgress;
        Vector3 minPos;
        Vector3 maxPos;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform currentChild = transform.GetChild(i);
            Quaternion originalChildRotation = currentChild.rotation;
            float progress = i / (float)transform.childCount;
            
            if (progress < corners[0])
            {
                partialProgress = Mathf.InverseLerp(0f, corners[0], progress);
                minPos = new Vector3(-scaledExtentsAxisConstrained.x, -scaledExtentsAxisConstrained.y, -scaledExtentsAxisConstrained.z);
                maxPos = new Vector3(scaledExtentsAxisConstrained.x, -scaledExtentsAxisConstrained.y, -scaledExtentsAxisConstrained.z);
            }
            else if (progress < corners[1])
            {
                partialProgress = Mathf.InverseLerp(corners[0], corners[1], progress);
                minPos = new Vector3(scaledExtentsAxisConstrained.x, -scaledExtentsAxisConstrained.y, -scaledExtentsAxisConstrained.z);
                maxPos = new Vector3(scaledExtentsAxisConstrained.x, scaledExtentsAxisConstrained.y, scaledExtentsAxisConstrained.z);
            }
            else if (progress < corners[2])
            {
                partialProgress = Mathf.InverseLerp(corners[1], corners[2], progress);
                minPos = new Vector3(scaledExtentsAxisConstrained.x, scaledExtentsAxisConstrained.y, scaledExtentsAxisConstrained.z);
                maxPos = new Vector3(-scaledExtentsAxisConstrained.x, scaledExtentsAxisConstrained.y, scaledExtentsAxisConstrained.z);
            }
            else
            {
                partialProgress = Mathf.InverseLerp(corners[2], corners[3], progress);
                minPos = new Vector3(-scaledExtentsAxisConstrained.x, scaledExtentsAxisConstrained.y, scaledExtentsAxisConstrained.z);
                maxPos = new Vector3(-scaledExtentsAxisConstrained.x, -scaledExtentsAxisConstrained.y, -scaledExtentsAxisConstrained.z);
            }
            Vector3 newPosition = Vector3.Lerp(minPos, maxPos, partialProgress);
            currentChild.localPosition = newPosition;

            SetChildRotation(currentChild, originalChildRotation, OverwriteChildrenRotation);
        }
    }

    public void DistributeCircular(Vector3 axes)
    {
        float radius = Radius;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform currentChild = transform.GetChild(i);
            Quaternion originalChildRotation = currentChild.rotation;
            float progress = i / (float)(transform.childCount);
            progress *= Mathf.PI * 2f;

            Vector3 newPos = new Vector3(Mathf.Sin(progress), Mathf.Cos(progress), Mathf.Cos(progress));
            newPos *= radius;
            newPos.Scale(axes);

            currentChild.localPosition = newPos;
            SetChildRotation(currentChild, originalChildRotation, OverwriteChildrenRotation);
        }
    }

    public void SetChildRotation(Transform child, Quaternion originalRotation, bool overwrite)
    {
        if (overwrite)
            child.localRotation = Quaternion.identity;
        else
            child.rotation = originalRotation;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 handlesPos = new Vector3();
        Handles.matrix = transform.localToWorldMatrix;
        float radius = Radius;

        Vector3 up = Vector3.up;
        Vector3 forward = Vector3.forward;

        switch (distribution)
        {
            case Distribution.CornerToCorner:
            case Distribution.RectangleXY:
            case Distribution.RectangleXZ:
            //case Distribution.RectangleYZ:
                Handles.DrawWireCube(handlesPos, Box);
                break;
            case Distribution.CircularXY:
                Handles.DrawWireDisc(handlesPos, forward, radius);
                break;
            case Distribution.CircularXZ:
                Handles.DrawWireDisc(handlesPos, up, radius);
                break;
        }
    }
#endif
}
