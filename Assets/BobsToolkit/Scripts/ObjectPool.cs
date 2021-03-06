﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes
{
    public class ObjectPool : MonoBehaviour
    {
        public GameObject prefab;

        [Min(1)]
        public int initialPoolSize = 10;
        [Tooltip("If ticked, the object pool will expand to fit the demands.")]
        public bool flexible = true;
        public List<GameObject> objectPool;

        public List<GameObject> GetObjectPool()
        {
            if (objectPool == null)
            {
                objectPool = new List<GameObject>();
            }
            if (objectPool.Count < initialPoolSize)
            {
                for (int i = objectPool.Count; i < initialPoolSize; i++)
                {
                    GameObject addition = Instantiate(prefab);
                    objectPool.Add(addition);
                    addition.SetActive(false);
                }
            }

            return objectPool;
        }

        public GameObject GetInactive()
        {
            List<GameObject> objPool = GetObjectPool();
            for (int i = 0; i < objPool.Count; i++)
            {
                if (!objPool[i].activeInHierarchy)
                {
                    //Debug.Log("Object at index " + i + " returned");
                    return objPool[i];
                }
            }
            if (flexible)
            {
                GameObject addition = Instantiate(prefab);
                objPool.Add(addition);

                Debug.Log("Object pool expanded to " + objPool.Count, this);

                addition.SetActive(false);
                return addition;
            }
            Debug.LogWarning("Object pool exhausted."
                + "Set to 'flexible' to allow dynamic expansion."
                );
            return null;
        }
    }
}