using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraFollow))]
public class CameraController : MonoBehaviour
{
    public float RotationSpeed = 1f;
    public float ZoomSpeed = .01f;
    public float ZoomMin = 2f;
    public float ZoomMax = 7f;

    private float rotationDelta = 0f;

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
        Debug.Log("Subscribe controls");
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
        Debug.Log("Unsubscribe controls");
        Controls.Game.Disable();
        Controls.Game.CameraRotationHorizontal.performed -= _ => SetRotationDelta(_.ReadValue<float>());
        Controls.Game.CameraRotationHorizontal.canceled -= _ => SetRotationDelta(0f);
        Controls.Game.CameraZoom.performed -= _ => SetZoomDelta(_.ReadValue<float>());
        controlsSubscribed = false;
    }

    private void SetRotationDelta(float input)
    {
        //Debug.Log("Set camera rotation delta: " + input);
        rotationDelta = input * RotationSpeed;
    }

    private void SetZoomDelta(float input)
    {
        CameraFollow.Distance = Mathf.Clamp(CameraFollow.Distance + (input * ZoomSpeed), ZoomMin, ZoomMax);
    }

    private void Update()
    {
        CameraFollow.RotationAroundTarget.y += rotationDelta;
    }
}
