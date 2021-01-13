using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TitleScreen : MonoBehaviour
{
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

    private void OnEnable()
    {
        Controls.TitleScreen.Enable();
        Controls.TitleScreen.Start.performed += _ => Click();
    }

    private void OnDisable()
    {
        Controls.TitleScreen.Disable();
        Controls.TitleScreen.Start.performed -= _ => Click();
    }

    public UnityEvent OnClick;
    public void Click()
    {
        OnClick.Invoke();
    }
}
