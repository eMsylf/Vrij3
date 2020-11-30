using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectsInsideTrigger : MonoBehaviour
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
                Debug.Log(name + " already contains " + obj.name, this);
            return;
        }
        gameObjects.Add(obj);
    }

    public void Remove(GameObject obj)
    {
        bool contains = gameObjects.Contains(obj);
        if (!contains)
        {
            Debug.LogError(name + " does not contain " + obj.name + ", therefore it cannot be removed from the trigger list", this);
            return;
        }
        int instancesRemoved = 0;
        if (allowDuplicates)
        {
            bool foundRemovable = true;
            while (foundRemovable)
            {
                foundRemovable = gameObjects.Remove(obj);
                instancesRemoved++;
            }
        }
        else
        {
            if (gameObjects.Remove(obj))
                instancesRemoved++;
        }

        Debug.Log(instancesRemoved + " instances of " + obj.name + " + removed", this);
    }
}
