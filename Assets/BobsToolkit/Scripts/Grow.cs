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
        Logarithmic
    }
    public Method method;

    public bool resetScaleOnEnable = true;
    public Vector3 referenceScale = Vector3.one;

    private void Update()
    {
        switch (method)
        {
            case Method.Multiply:
                transform.localScale = Vector3.Scale(transform.localScale, growth);
                break;
            case Method.Add:
                transform.localScale += growth;
                break;
            case Method.Logarithmic:
                break;
        }
    }

    private void OnEnable()
    {
        if (resetScaleOnEnable)
            transform.localScale = referenceScale;
    }
}
