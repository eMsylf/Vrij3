using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public float Time = 3f;
    public void OnEnable()
    {
        Destroy(gameObject, Time);
    }
}
