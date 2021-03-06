﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float Speed = 6.944444e-05f;

    void Update()
    {
        transform.Rotate(transform.right, Speed * Time.deltaTime, Space.World);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.DrawWireDisc(transform.position, transform.right, 2f);
    }
#endif
}
