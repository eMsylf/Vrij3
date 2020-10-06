using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    public Transform Target
    {
        get
        {
            if (target == null)
            {
                Debug.LogWarning("No target assigned in " + name, this);
                return transform;
            }
            return target;
        }
        set => target = value;
    }

    [SerializeField] private Transform cameraResources;
    public Transform CameraResources
    {
        get
        {
            if (cameraResources == null)
            {
                cameraResources = new GameObject("Camera resources").transform;
            }
            return cameraResources;
        }
    }

    [Space]

    [SerializeField] private Transform positionPivot;
    public Transform PositionPivot
    {
        get
        {
            if (positionPivot == null)
            {
                positionPivot = new GameObject("Position pivot").transform;
                positionPivot.parent = CameraResources;
            }
            return positionPivot;
        }
    }

    [SerializeField] private Transform positionTransform;
    public Transform PositionTransform
    {
        get
        {
            if (positionTransform == null)
            {
                positionTransform = new GameObject("Position transform").transform;
                positionTransform.parent = PositionPivot;
            }
            return positionTransform;
        }
    }

    [SerializeField] private Transform lookTransform;
    public Transform LookTransform
    {
        get
        {
            if (lookTransform == null)
            {
                lookTransform = new GameObject("Look transform").transform;
                lookTransform.parent = CameraResources;
            }
            return lookTransform;
        }
    }
    [Header("Position")]
    public Vector3 PositionOffset;
    [Range(0f, .99f)]
    public float PositionSmoothing = .125f;

    [Header("Rotation")]
    public Vector3 RotationAroundTarget;
    [Header("Distance")]
    [Min(0f)]
    public float Distance = 2f;
    [Header("Look")]
    public Vector3 LookOffset;
    [Range(0f, .99f)]
    public float LookSmoothing = .125f;

    [Header("Live preview")]
    public bool LivePreview = true;

    private void Update()
    {
        SetPositionAndLook();
    }

    public void SetPositionAndLook()
    {
        CalculatePivot();

        SetPosition(CalculatePosition());

        CalculateRotationAroundTarget();

        SetLook(CalculateLook());
    }

    public void CalculatePivot()
    {
        PositionPivot.position = Target.position + PositionOffset;
    }

    public Vector3 CalculatePosition()
    {
        PositionTransform.position = PositionPivot.position + PositionPivot.forward * -Distance;
        return PositionTransform.position;
    }

    private void SetPosition(Vector3 position)
    {
        transform.position = Vector3.Lerp(transform.position, position, 1f - PositionSmoothing);
    }

    private void CalculateRotationAroundTarget()
    {
        PositionPivot.rotation = Quaternion.Euler(RotationAroundTarget);
    }

    private Vector3 CalculateLook()
    {
        // Get camera look position
        Vector3 desiredLook = Target.position + LookOffset;
        if (LookSmoothing == 0f)
            LookTransform.position = desiredLook;
        else
            LookTransform.position = Vector3.Lerp(lookTransform.position, desiredLook, 1f - LookSmoothing);
        return LookTransform.position;
    }

    private void SetLook(Vector3 position)
    {
        // Camera looks at the target position
        transform.LookAt(position);
    }

    //public void OverrideOffsets()
    //{
    //    //PositionTransform.position = transform.position;
    //    PositionOffset = Target.position - transform.position;
    //}

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 targetPos = Target.position;

        Gizmos.DrawLine(transform.position, targetPos);
        Gizmos.DrawLine(PositionTransform.position, targetPos);
        Gizmos.DrawLine(LookTransform.position, targetPos);
        Gizmos.DrawLine(PositionTransform.position, PositionPivot.position);
        Gizmos.DrawLine(PositionPivot.position, Target.position);
    }
#endif
}
