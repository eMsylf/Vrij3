using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDirector : MonoBehaviour
{
    [Tooltip("Uses the main camera by default")]
    public Camera OverrideCamera;

    private Camera cam;
    private Camera GetCamera()
    {
        cam = OverrideCamera == null ? Camera.main : OverrideCamera;
        return cam;
    }
}
