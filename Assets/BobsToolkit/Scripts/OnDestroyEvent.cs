using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnDestroyEvent : MonoBehaviour
{
    public UnityEvent onDestroy = new UnityEvent();
    private void OnDestroy()
    {
        
        if (onDestroy != null && onDestroy.GetPersistentEventCount() > 0)
            onDestroy.Invoke();
    }
}
