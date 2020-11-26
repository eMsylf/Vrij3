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
    private List<GameObject> objectsInside = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (layers != (layers.value | (1 << other.gameObject.layer)))
        {
            return;
        }
        objectsInside.Add(other.gameObject);
        onTriggerEnter.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (layers != (layers.value | (1 << other.gameObject.layer)))
        {
            return;
        }
        objectsInside.Remove(other.gameObject);
        onTriggerExit.Invoke();
    }


    private void OnDisable()
    {
        if (exitOnDisable)
        {
            for (int i = 0; i < objectsInside.Count; i++)
            {
                onTriggerExit.Invoke();
            }
        }
        objectsInside.Clear();
    }
}
