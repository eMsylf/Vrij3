using BobJeltes.StandardUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CameraFollow))]
public class CameraController : Singleton<CameraController>
{
    public float RotationSpeed = 1f;
    public float ZoomSpeed = .01f;
    public float ZoomMin = 2f;
    public float ZoomMax = 20f;
    public float xRotationMax = 20f;

    private Vector2 rotationDelta = new Vector2();

    Controls controls;
    Controls Controls
    {
        get
        {
            if (controls == null)
            {
                controls = new Controls();
            }
            return controls;
        }
    }

    CameraFollow cameraFollow;
    CameraFollow CameraFollow
    {
        get
        {
            if (cameraFollow == null)
            {
                cameraFollow = GetComponent<CameraFollow>();
            }
            return cameraFollow;
        }
    }

    private void OnEnable()
    {
        Controls.Game.Enable();
    }

    private void OnDisable()
    {
        Controls.Game.Disable();
    }

    private Vector3 GetRotationDelta(Vector2 input)
    {
        if (input == Vector2.zero)
        {
            return Vector3.zero;
        }

        // Swap the axes (y should govern the x-axis rotation, and vice versa)
        float x = input.x;
        input.x = input.y;
        input.y = x;

        rotationDelta = input * RotationSpeed;

        return rotationDelta;
    }

    private float GetZoomDelta(float input)
    {
        //Debug.Log("Zoom input: " + input);
        return input * ZoomSpeed;
    }

    private void Update()
    {
        CameraFollow.RotationAroundTarget += GetRotationDelta(Controls.Game.Look.ReadValue<Vector2>());
        CameraFollow.RotationAroundTarget.x = Mathf.Clamp(CameraFollow.RotationAroundTarget.x, -xRotationMax, xRotationMax);

        CameraFollow.Distance += GetZoomDelta(Controls.Game.CameraZoom.ReadValue<float>());
        CameraFollow.Distance = Mathf.Clamp(CameraFollow.Distance, ZoomMin, ZoomMax);
    }

    public void MultiplyZoomConstraints(float factor)
    {
        ZoomMin *= factor;
        ZoomMax *= factor;
        CameraFollow.Distance *= factor;
    }
}
