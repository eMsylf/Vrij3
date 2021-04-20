using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public bool Continuous;
    public bool PlayOnAwake = true;
    public bool Local;
    [Header("Wave settings")]
    [Min(0)]
    public int Amount;
    [Min(0)]
    public float Interval = 1f;
    private float timeRemaining;
    public UnityEvent OnSpawn;

    public void Spawn()
    {
        Debug.Log("Spawn");
        for (int i = 0; i < Amount; i++)
        {
            if (Local) Instantiate(prefab, transform);
            else Instantiate(prefab, transform.position, transform.rotation);
        }
        OnSpawn.Invoke();
    }

    private void OnEnable()
    {
        if (PlayOnAwake) Spawn();
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
