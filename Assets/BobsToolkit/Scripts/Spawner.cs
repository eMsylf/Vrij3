using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public bool Continuous;

    [Header("Wave settings")]
    [Min(0)]
    public int Amount;
    [Min(0)]
    public float Interval = 1f;
    private float timeRemaining;

    public void Spawn()
    {
        for (int i = 0; i < Amount; i++)
        {
            Instantiate(prefab, transform);
        }
    }

    private void Update()
    {
        if (Continuous)
        {
            if (timeRemaining <= 0f)
            {
                timeRemaining = Interval;
                Spawn();
                return;
            }
            timeRemaining -= Time.deltaTime;
        }
    }

    public IEnumerator SpawnFor(float time)
    {
        Continuous = true;
        yield return new WaitForSeconds(time);
        Continuous = false;
    }
}
