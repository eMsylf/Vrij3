using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerList : ListKeeper
{
    public LayerMask layers;

    private void OnTriggerEnter(Collider other)
    {
        if (layers != (layers.value | (1 << other.gameObject.layer)))
        {
            return;
        }
        Debug.Log(other.name + " entered trigger of " + name, this);
        
        Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (layers != (layers.value | (1 << other.gameObject.layer)))
        {
            return;
        }
        Debug.Log(other.name + " left trigger of " + name, this);
        Remove(other.gameObject);
    }
}
