using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraFollow))]
public class CameraController : MonoBehaviour
{
    public float RotationSpeed = 1f;
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
        controls.Game.CameraRotationHorizontal.canceled += _ => SetRotationDelta(0f);
        controlsSubscribed = true;
    }

    private void UnsubControls()
    {
        if (!controlsSubscribed)
            return;
        Debug.Log("Unsubscribe controls");
        Controls.Game.Disable();
        Controls.Game.CameraRotationHorizontal.performed -= _ => SetRotationDelta(_.ReadValue<float>());
        controls.Game.CameraRotationHorizontal.canceled -= _ => SetRotationDelta(0f);
        controlsSubscribed = false;
    }

    private void SetRotationDelta(float input)
    {
        //Debug.Log("Set camera rotation delta: " + input);
        rotationDelta = input * RotationSpeed;
    }

    private void Update()
    {
        CameraFollow.RotationAroundTarget.y += rotationDelta;
    }
}
