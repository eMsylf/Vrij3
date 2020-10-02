using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationHorizontal : MonoBehaviour {
    [SerializeField] private float rotationSpeed = 1f;
    [Range(.1f, .9f)]
    [SerializeField] private float rotationSmoothing = .9f;
    private float cameraRotationY;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerArt = null;
    [SerializeField] private Quaternion storedRotation;
    [SerializeField] private float horizontalR;
    [SerializeField] private float vertical;
    [SerializeField] private float horizontal;

    [Range(.01f, 1f)]
    [SerializeField] private float playerRotationSmoothing = .33f;

    void Update() {
        // Camera rotation
        horizontalR = Input.GetAxis("HorizontalR");

        cameraRotationY = Mathf.Lerp(
            cameraRotationY, 
            horizontalR * rotationSpeed, 
            rotationSmoothing
            );
        transform.Rotate(0f, cameraRotationY, 0f, Space.Self);

        //Debug.Log(Input.GetAxis("HorizontalR"));

        // Player rotation
        Vector3 forwardDirectionCamera = Vector3.Scale(transform.forward, new Vector3(1, 0, 1));

        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");

        if (vertical == 0f && horizontal == 0f) {
            playerArt.rotation = storedRotation;
        } else {
            playerArt.rotation = Quaternion.Lerp(
                storedRotation, 
                Quaternion.LookRotation(forwardDirectionCamera * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal")), 
                playerRotationSmoothing);
        }
        storedRotation = playerArt.rotation;
        //StoreLookRotation(playerArt.rotation);

    }

    private void StoreLookRotation(Quaternion quaternion) {
        storedRotation = quaternion;
    }
}
