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
        Curve
    }
    public Method method;
    public AnimationCurve growthCurve = new AnimationCurve();

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
            case Method.Curve:
                progress += Time.fixedDeltaTime;
                float evaluatedProgress = growthCurve.Evaluate(progress);
                transform.localScale =  Vector3.Lerp(referenceScale, growth, evaluatedProgress);
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
