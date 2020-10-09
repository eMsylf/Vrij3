using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CanEditMultipleObjects]
#endif
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

        if (StayUpright) targetPosition.y = transform.position.y;

        lookVector = transform.position - targetPosition;
        if (reverse) lookVector *= -1f;
        switch (Method)
        {
            case EMethod.Forward:
                transform.forward = lookVector;
                break;
        };
    }
}
