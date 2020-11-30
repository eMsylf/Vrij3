using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public LayerMask layers;
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;
    public bool exitOnDisable = true;

    private void OnTriggerEnter(Collider other)
    {
        if (layers != (layers.value | (1 << other.gameObject.layer)))
        {
            return;
        }
        Debug.Log(other.name + " entered trigger of " + name, this);
        onTriggerEnter.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (layers != (layers.value | (1 << other.gameObject.layer)))
        {
            return;
        }
        Debug.Log(other.name + " left trigger of " + name, this);
        onTriggerExit.Invoke();
    }


    private void OnDisable()
    {
        onTriggerExit.Invoke();
    }
}
