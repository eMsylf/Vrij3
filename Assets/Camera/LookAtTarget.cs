using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    [Tooltip("Uses the main camera by default")]
    public Transform OverrideTarget;

    private Transform target;
    private Transform GetTarget()
    {
        target = OverrideTarget == null ? Camera.main.transform : OverrideTarget;
        return target;
    }

    public enum EFacingSide
    {
        AlignRotation = default,
        Forward,
        Up,
        Right
    }
    public EFacingSide FacingSide = default;
    [Tooltip("Only takes effect when Forward, Up or Right is selected under FacingSide")]
    public bool reverse;
    private Vector3 lookVector;

    private void Update()
    {
        switch (FacingSide)
        {
            case EFacingSide.AlignRotation:
                transform.rotation = GetTarget().transform.rotation;
                return;
        }

        Vector3 targetPosition = GetTarget().transform.position;
        Debug.DrawLine(transform.position, targetPosition, Color.white, 1f);

        lookVector = transform.position - targetPosition;
        if (reverse) lookVector *= -1f;
        switch (FacingSide)
        {
            case EFacingSide.Forward:
                transform.forward = lookVector;
                break;
            case EFacingSide.Up:
                transform.up = lookVector;
                break;
            case EFacingSide.Right:
                transform.right = lookVector;
                break;
        };
    }
}
