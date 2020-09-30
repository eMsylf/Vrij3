using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationVertical : MonoBehaviour {
    [SerializeField] private float cameraRotationVert;

    [SerializeField] private float rotationSpeed = 1f;
    [Range(.1f, .9f)]
    [SerializeField] private float rotationSmoothing = .9f;
    [SerializeField] private bool invertVertical;
    [SerializeField] private float rotationUpperBound;
    [SerializeField] private float rotationLowerBound;

    public Vector3 rot;


    void Start() {
        cameraRotationVert = 0f;
    }

    void Update() {
        cameraRotationVert = Mathf.Lerp(
            cameraRotationVert, 
            Input.GetAxis("VerticalR") * rotationSpeed, 
            rotationSmoothing
            );
        if (invertVertical) {
            cameraRotationVert *= -1;
        }

        //if (transform.rotation.x + cameraRotationVert < rotationLowerBound) {
        //    return;
        //}

        transform.Rotate(cameraRotationVert, 0f, 0f, Space.Self); // Rotate the camera

        rot = transform.eulerAngles; // Store current rotation after input

        rot.x = Mathf.Clamp(rot.x, rotationLowerBound, rotationUpperBound); // Clamp x-axis rotation

        //transform.eulerAngles = rot; // Return clamped x-axis rotation to the transform

        



    }

    
}
