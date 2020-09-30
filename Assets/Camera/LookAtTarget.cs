using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
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

    public enum EMethod
    {
        AlignRotation = default,
        Forward
        //Up,
        //Right
    }
    public EMethod Method = default;
    [Tooltip("Only takes effect when Forward is selected")]
    public bool reverse;
    private Vector3 lookVector;
    [Tooltip("Only takes effect when Forward is selected")]
    public bool StayUpright;

    private void Update()
    {
        switch (Method)
        {
            case EMethod.AlignRotation:
                transform.rotation = GetTarget().transform.rotation;
                return;
        }

        Vector3 targetPosition = GetTarget().transform.position;
        Debug.DrawLine(transform.position, targetPosition, Color.white, 1f);

        //switch (FacingSide)
        //{
        //    case EFacingSide.Forward:
        //        if (StayUpright) targetPosition.y = transform.position.y;
        //        break;
        //    case EFacingSide.Up:
        //        if (StayUpright) targetPosition.z = transform.position.z;
        //        break;
        //    case EFacingSide.Right:
        //        if (StayUpright) targetPosition.x = transform.position.x;
        //        break;
        //}
        if (StayUpright) targetPosition.y = transform.position.y;

        lookVector = transform.position - targetPosition;
        if (reverse) lookVector *= -1f;
        switch (Method)
        {
            case EMethod.Forward:
                transform.forward = lookVector;
                break;
            //case EMethod.Up:
            //    transform.up = lookVector;
            //    break;
            //case EMethod.Right:
            //    transform.right = lookVector;
            //    break;
        };
    }
}
