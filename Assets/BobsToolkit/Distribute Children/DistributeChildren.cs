using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DistributeChildren : MonoBehaviour
{
    public Vector3 Extents = Vector3.one;
    public float Radius = 1f;
    //public bool DistributionInheritsScale = false;
    //public bool DistributionInheritsRotation = false;
    public bool OverwriteChildrenRotation = false;

    public Vector3 GetScaledExtents()
    {
        //if (DistributionInheritsScale)
        //{
        //    return Vector3.Scale(Extents, transform.localScale);
        //}
        //else
        //{
        //    return Extents;
        //}
        return Extents;
    }
    public Vector3 ScaledHalfExtents => GetScaledExtents() / 2f;

    //public float GetScaledRadius()
    //{
    //    return GetScaledRadius(DistributionInheritsScale);
    //}

    public float GetScaledRadius(bool scaled)
    {
        //return scaled ? Radius: Radius / transform.localScale.magnitude;
        if (scaled)
        {
            // Get scaled radius
            return Radius;
        }
        else
        {
            // Get unscaled radius
            return Radius;
        }
    }

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
            Quaternion originalChildRotation = currentChild.rotation;
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

            SetChildRotation(currentChild, originalChildRotation, OverwriteChildrenRotation);
        }
    }

    public void DistributeCircular(Vector3 axes)
    {
        float radius = Radius;
        Debug.Log("Scaled radius: " + radius);
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform currentChild = transform.GetChild(i);
            Quaternion originalChildRotation = currentChild.rotation;
            float progress = i / (float)(transform.childCount);
            progress *= Mathf.PI * 2f;

            //Debug.Log("i = " + i + "--- Sine = " + Mathf.Sin(i));
            Vector3 newPos = new Vector3(Mathf.Sin(progress), Mathf.Cos(progress), Mathf.Cos(progress));
            newPos *= radius;
            //newPos /= Radius;
            newPos.Scale(axes);

            //if (DistributionInheritsScale)
            //{
            currentChild.localPosition = newPos;
            //}
            //else
            //{
            //    currentChild.position = transform.position + newPos;
            //    //currentChild.position = transform.position + newPos;
            //}

            SetChildRotation(currentChild, originalChildRotation, OverwriteChildrenRotation);
        }
    }

    public void SetChildRotation(Transform child, Quaternion originalRotation, bool overwrite)
    {
        if (overwrite)
        {
            child.localRotation = Quaternion.identity;
        }
        else
        {
            child.rotation = originalRotation;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        //Debug.Log("Gizmo matrix: " + Gizmos.matrix);
        //Debug.Log("Gizmo matrix: " + Gizmos.matrix);
        Vector3 handlesPos = new Vector3();
        Handles.matrix = transform.localToWorldMatrix;
        //if (DistributionInheritsScale)
        //{
        //    handlesPos = transform.position;
        //}
        Vector3 extents = GetScaledExtents();
        float radius = Radius;
        //if (!DistributionInheritsScale)
        //{
        //    extents = ScaledHalfExtents;
        //}

        Vector3 up = Vector3.up;
        Vector3 forward = Vector3.forward;
        //if (DistributionInheritsRotation)
        //{
        //    up = transform.up;
        //    forward = transform.forward;
        //}

        //Quaternion rotation = Quaternion.identity;
        //if (ObjectsInheritRotation)
        //{
        //    rotation = transform.rotation;
        //}

        switch (distribution)
        {
            case Distribution.CornerToCorner:
            case Distribution.RectangleXY:
            case Distribution.RectangleXZ:
            //case Distribution.RectangleYZ:
                Handles.DrawWireCube(handlesPos, extents);
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
