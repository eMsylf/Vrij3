using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Transform CameraViewPoint;

    private void OnTriggerEnter(Collider other)
    {
        Camera.main.transform.position = CameraViewPoint.position;
        Camera.main.transform.rotation = CameraViewPoint.rotation;
    }
}
