using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEvent : MonoBehaviour
{
    public LayerMask layers;
    public UnityEvent CollisionEffect;

    private void OnCollisionEnter(Collision collision)
    {
        if (layers != (layers.value | (1 << collision.gameObject.layer)))
        {
            return;
        }
        CollisionEffect.Invoke();
    }
}
