﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Transform CameraViewPoint;
    public GameObject PhysicalRoom;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null)
            return;
        Debug.Log(other.name + " entered " + name + ". Activate.", this);
        PhysicalRoom?.SetActive(true);
        //Debug.Log("Set camera position to " + name, this);
        Camera.main.transform.position = CameraViewPoint.position;
        Camera.main.transform.rotation = CameraViewPoint.rotation;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null)
            return;
        Debug.Log(other.name + " left " + name + ". Deactivate.", this);
        PhysicalRoom?.SetActive(false);
    }
}
