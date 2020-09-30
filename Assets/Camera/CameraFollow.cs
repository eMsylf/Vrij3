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
        set
        {
            target = value;
        }
    }
    public Vector3 PositionOffset;
    public Quaternion PositionOffsetRot = Quaternion.identity;
    public float Distance = 1f;
    [Range(.1f, 1f)]
    public float PositionSmoothing = .125f;
    public Vector3 LookOffset;
    public Quaternion LookOffsetRot = Quaternion.identity;
    [Range(.1f, 1f)]
    public float LookSmoothing = .125f;
    public bool LivePreview = true;

    public float HorizontalRotation = 0f;

    private void Update()
    {
        SetPositionAndLook();
    }

    public void SetPositionAndLook()
    {
        SetPosition(GetMoveTargetPosition());

        CalculateRotationAroundTarget();

        SetLook(GetLookTargetPosition());
    }

    public Vector3 GetMoveTargetPosition()
    {
        Vector3 movePosition;
        if (PositionSmoothing == 0f)
        {
            movePosition = Target.position + PositionOffset;
        }
        else
        {
            movePosition = Vector3.Lerp(transform.position, Target.position + PositionOffset, 1f - PositionSmoothing);
        }

        return movePosition;
    } 

    private void CalculateRotationAroundTarget()
    {
        Vector3 currentOffset = Target.position - transform.position;

        Distance = Vector3.Distance(Target.position, transform.position);

        currentOffset.x = currentOffset.x * Mathf.Cos(HorizontalRotation);
        currentOffset.z = Mathf.Cos(HorizontalRotation);

        transform.position = currentOffset + Target.position;
    }

    private Vector3 GetLookTargetPosition()
    {
        // Get camera look position
        if (LookSmoothing == 0f)
        {
            return Target.position;
        }
        return Vector3.Lerp(transform.position, Target.position + LookOffset, 1f - LookSmoothing);
    }

    private void SetPosition(Vector3 position)
    {
        // Set camera position
        transform.position = position;
    }

    private void SetLook(Vector3 position)
    {
        // Camera looks at the target position
        transform.LookAt(position);
    }

    public void OverrideOffsets()
    {
        PositionOffset = Target.position - transform.position;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 targetPos = Target.position;

        Gizmos.DrawLine(transform.position, targetPos);
        Gizmos.DrawCube(targetPos + PositionOffset, Vector3.one);
    }
#endif
}
