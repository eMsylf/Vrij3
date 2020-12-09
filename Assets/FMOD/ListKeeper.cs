using BobJeltes.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ListKeeper : MonoBehaviour
{
    public List<GameObject> gameObjects = new List<GameObject>();

    public bool allowDuplicates;

    public UnityEvent OnAdd;
    public UnityEvent OnRemove;

    public void Add(GameObject obj)
    {
        bool contains = gameObjects.Contains(obj);
        if (contains)
        {
            if (!allowDuplicates)
            {
                return;
                //Debug.Log(name + " already contains " + obj.name, this);
            }
        }
        gameObjects.Add(obj);
        OnAdd.Invoke();
        
        // Automatically remove on disable
        OnDisableEvent disableEvent = obj.GetComponent<OnDisableEvent>();
        if (disableEvent == null)
            disableEvent = obj.AddComponent<OnDisableEvent>();

        disableEvent.onDisable.AddListener(() => Remove(obj));
    }

    public void Remove(GameObject obj)
    {
        bool contains = gameObjects.Contains(obj);
        if (!contains)
        {
            //Debug.LogWarning(name + " does not contain " + obj.name + ", therefore it cannot be removed from the trigger list", this);
            return;
        }
        int instancesRemoved = 0;
        if (allowDuplicates)
        {
            while (true)
            {
                if (gameObjects.Remove(obj))
                {
                    OnRemove.Invoke();
                    instancesRemoved++;
                    continue;
                }
                break;
            }
        }
        else
        {
            if (gameObjects.Remove(obj))
            {
                OnRemove.Invoke();
            }

            instancesRemoved++;
        }

        //Debug.Log(instancesRemoved + " instances of " + obj.name + " + removed from " + name, this);
    }

    public void Clear()
    {
        while (gameObjects.Count > 0)
            Remove(gameObjects[0]);
    }
}
