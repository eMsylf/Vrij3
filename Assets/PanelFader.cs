using BobJeltes.Menu;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using DG.Tweening;

public class PanelFader : MonoBehaviour
{

    public bool UndoFadeOnEnable = true;

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

        if (UndoFadeOnEnable)
            FadeUndo();
    }

    private void OnDisable()
    {
        Controls.TitleScreen.Disable();
        Controls.TitleScreen.Start.performed -= _ => Fade();
    }

    public void Fade() {
        var canvGroup = GetComponent<CanvasGroup>();
        OnFadeStart.Invoke();

        canvGroup.DOFade(0f, duration).onComplete += () => OnFadeEnd.Invoke();
    }

    public void FadeUndo()
    {
        var canvGroup = GetComponent<CanvasGroup>();

        canvGroup.alpha = 1f;
    }
}
