using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicAttack : MonoBehaviour
{
    public GameObject attackPrefab;

    [Min(0)]
    public float Interval = 1f;
    private float interval;

    private void Update()
    {
        if (interval > 0f)
        {
            interval -= Time.deltaTime;
            return;
        }

        interval = Interval;

        Debug.Log("Launch attack");
        Instantiate(attackPrefab, transform);
    }
}
