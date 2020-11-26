using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grow : MonoBehaviour
{
    public Vector3 growth = Vector3.one;

    public enum Method
    {
        Multiply,
        Add,
        GrowTowardsLinear
    }
    public Method method;
    public float speed = 1f;
    [Space]
    public bool resetScaleOnEnable = true;
    public Vector3 referenceScale = Vector3.one;

    private float progress = 0f;

    private void FixedUpdate()
    {
        switch (method)
        {
            case Method.Multiply:
                transform.localScale = Vector3.Scale(transform.localScale, growth);
                break;
            case Method.Add:
                transform.localScale += growth;
                break;
            case Method.GrowTowardsLinear:
                transform.localScale = Vector3.Lerp(transform.localScale, growth, progress);
                progress += Time.fixedDeltaTime * speed;
                progress = Mathf.Clamp(progress, 0f, 1f);
                break;
        }
    }

    private void OnEnable()
    {
        if (resetScaleOnEnable)
        {
            progress = 0f;
            transform.localScale = referenceScale;
        }
    }
}
