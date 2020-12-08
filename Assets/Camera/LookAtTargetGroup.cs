using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTargetGroup : MonoBehaviour
{
    [Tooltip("Uses the main camera by default")]
    public Transform OverrideTarget;

    private Transform target;
    private Transform GetTarget()
    {
        target = OverrideTarget == null ? Camera.main.transform : OverrideTarget;
        return target;
    }

    public enum Method
    {
        AlignRotation = default,
        Forward
    }
    public Method method = default;
    [Header("Only works with 'Forward'")]
    public bool reverse;
    public bool StayUpright;

    private void Update()
    {
        switch (method)
        {
            case Method.AlignRotation:
                foreach (Transform trans in transform)
                {
                    trans.forward = GetTarget().forward;
                }
                break;

            case Method.Forward:
                Vector3 targetPosition = GetTarget().position;

                foreach (Transform trans in transform)
                {
                    Vector3 lookVector = trans.position - targetPosition;
                    if (StayUpright) lookVector.y = 0f;
                    if (reverse) lookVector *= -1f;
                    trans.forward = lookVector;
                }
                break;
        }
    }
}
