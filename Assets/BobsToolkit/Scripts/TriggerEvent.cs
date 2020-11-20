using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public LayerMask layers;
    public UnityEvent triggerEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (layers != (layers.value | (1 << other.gameObject.layer)))
        {
            return;
        }
        triggerEffect.Invoke();
    }
}
