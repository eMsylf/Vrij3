using BobJeltes.StandardUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraFollow))]
public class CameraController : Singleton<CameraController>
{
    public float RotationSpeed = 1f;
    public float ZoomSpeed = .01f;
    public float ZoomMin = 2f;
    public float ZoomMax = 7f;

    private float rotationDelta = 0f;

    public bool AllowControls = true;

    public void EnableControlsInstance(bool enabled)
    {
        Instance.AllowControls = enabled;
    }

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
        SubscribeControls();
    }

    private void OnDisable()
    {
        UnsubControls();
    }

    bool controlsSubscribed;
    private void SubscribeControls()
    {
        if (controlsSubscribed)
            return;
        Debug.Log("Subscribe controls from " + name);
        Controls.Game.Enable();
        Controls.Game.CameraRotationHorizontal.performed += _ => SetRotationDelta(_.ReadValue<float>());
        Controls.Game.CameraRotationHorizontal.canceled += _ => SetRotationDelta(0f);
        Controls.Game.CameraZoom.performed += _ => SetZoomDelta(_.ReadValue<float>());
        controlsSubscribed = true;
    }

    private void UnsubControls()
    {
        if (!controlsSubscribed)
            return;
        Debug.Log("Unsubscribe controls from " + name);
        Controls.Game.Disable();
        Controls.Game.CameraRotationHorizontal.performed -= _ => SetRotationDelta(_.ReadValue<float>());
        Controls.Game.CameraRotationHorizontal.canceled -= _ => SetRotationDelta(0f);
        Controls.Game.CameraZoom.performed -= _ => SetZoomDelta(_.ReadValue<float>());
        controlsSubscribed = false;
    }

    private void SetRotationDelta(float input)
    {
        //Debug.Log("Set camera rotation delta: " + input);
        if (!AllowControls)
            return;
        rotationDelta = input * RotationSpeed;
    }

    private void SetZoomDelta(float input)
    {
        if (!AllowControls)
            return;
        CameraFollow.Distance = Mathf.Clamp(CameraFollow.Distance + (input * ZoomSpeed), ZoomMin, ZoomMax);
    }

    private void Update()
    {
        if (!AllowControls)
            return;
        CameraFollow.RotationAroundTarget.y += rotationDelta;
    }
}
