using BobJeltes.Menu;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class PanelFader : MonoBehaviour
{

    private bool mFaded = false;

    public float duration = 0.4f;
    public UnityEvent OnFadeStart;
    public UnityEvent OnFadeEnd;

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
        Controls.TitleScreen.Start.performed += _ => Fade();
        
    }

    private void OnDisable()
    {
        Controls.TitleScreen.Disable();
        Controls.TitleScreen.Start.performed -= _ => Fade();
    }

    public void Fade() {
        var canvGroup = GetComponent<CanvasGroup>();
        OnFadeStart.Invoke();
        StartCoroutine(DoFade(canvGroup, canvGroup.alpha, mFaded ? 1 : 0));
    }

    public IEnumerator DoFade(CanvasGroup canvGroup, float start, float end) {

        float counter = 0f;

        while (counter < duration) {

            counter += Time.deltaTime;
            canvGroup.alpha = Mathf.Lerp(start, end, counter / duration);

            yield return null;
        }

        OnFadeEnd.Invoke();
    
    }

}
