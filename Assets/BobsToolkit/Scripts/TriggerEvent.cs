using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public LayerMask layers;
    //public UnityEvent onTriggerEnter;
    public UnityEventGameObject onTriggerEnter;
    //public UnityEvent onTriggerExit;
    public UnityEventGameObject onTriggerExit;
    public bool onDisableExit = true;

    private void OnTriggerEnter(Collider other)
    {
        if (layers != (layers.value | (1 << other.gameObject.layer)))
        {
            return;
        }
        Debug.Log(other.name + " entered trigger of " + name, this);
        onTriggerEnter.Invoke(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (layers != (layers.value | (1 << other.gameObject.layer)))
        {
            return;
        }
        Debug.Log(other.name + " left trigger of " + name, this);
        onTriggerExit.Invoke(other.gameObject);
    }

    [System.Serializable]
    public class UnityEventGameObject : UnityEvent<GameObject>
    {

    }
}
