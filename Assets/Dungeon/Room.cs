using RanchyRats.Gyrus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Tooltip("When the room's trigger is entered, the main camera's position and rotation will be set to this transform")]
    public Transform CameraViewPoint;
    [Tooltip("Objects under this object will be turned off when the room's trigger is exited")]
    public GameObject PhysicalRoom;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() == null)
            return;
        Debug.Log(other.name + " entered " + name + ". Activate.", this);
        if (PhysicalRoom != null)
            PhysicalRoom.SetActive(true);
        //Debug.Log("Set camera position to " + name, this);
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
        if (PhysicalRoom != null)
            PhysicalRoom.SetActive(false);
    }
}
