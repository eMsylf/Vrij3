using BobJeltes.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    [Min(0)]
    public int Amount;
    public bool Local;
    [ShowIf("Local", false)]
    public Vector3 StartPosition = Vector3.zero;
    public Vector3 StartRotation = Vector3.zero;
    public bool SpawnOnEnable = true;
    [Header("Wave settings")]
    public bool Continuous;
    [Min(0)]
    public float Interval = 1f;
    private float timeRemaining;
    public UnityEvent OnSpawn;

    [ContextMenu("Spawn")]
    public void Spawn()
    {
        Debug.Log("Spawn");
        for (int i = 0; i < Amount; i++)
        {
            if (Local) Instantiate(prefab, transform);
            else
            {
                Instantiate(prefab, transform.position + StartPosition, transform.rotation * Quaternion.Euler(StartRotation));
            }
        }
        OnSpawn.Invoke();
    }

    private void OnEnable()
    {
        if (SpawnOnEnable) Spawn();
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
