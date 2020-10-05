using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Supply a transform to override the camera position upon entering the room")]
    public Transform CameraViewPoint;
    public GameObject RoomObjects;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null)
            return;
        Debug.Log(other.name + " entered " + name + ". Activate.", this);
        RoomObjects?.SetActive(true);
        if (CameraViewPoint != null)
        {
            Camera.main.transform.position = CameraViewPoint.position;
            Camera.main.transform.rotation = CameraViewPoint.rotation;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null)
            return;
        Debug.Log(other.name + " left " + name + ". Deactivate.", this);
        RoomObjects?.SetActive(false);
    }
}
