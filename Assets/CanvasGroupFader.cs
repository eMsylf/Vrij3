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

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupFader : MonoBehaviour
{

    public bool UndoFadeOnEnable = true;

    public float duration = 0.4f;
    public float undoDuration = 0.1f;
    public UnityEvent OnFade;
    public UnityEvent OnFadeEnd;
    public UnityEvent OnFadeUndo;
    public UnityEvent OnFadeUndoEnd;

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
        Fade(duration);
    }

    public void Fade(float _duration)
    {
        var canvGroup = GetComponent<CanvasGroup>();
        OnFade.Invoke();
        if (duration <= 0f)
        {
            canvGroup.alpha = 0f;
            OnFadeEnd.Invoke();
        }
        else
        {
            canvGroup.alpha = 1f;
            canvGroup.DOFade(0f, _duration).onComplete += () => OnFadeEnd.Invoke();
        }
    }

    public void FadeUndo()
    {
        FadeUndo(undoDuration);
    }

    public void FadeUndo(float _duration)
    {
        Debug.Log("Undo fade: " + name, this);
        var canvGroup = GetComponent<CanvasGroup>();
        OnFadeUndo.Invoke();
        if (undoDuration <= 0f)
        {
            canvGroup.alpha = 1f;
            OnFadeUndoEnd.Invoke();
        }
        else
        {
            canvGroup.alpha = 0f;
            canvGroup.DOFade(1f, _duration).onComplete += () => OnFadeUndoEnd.Invoke();
        }
    }
}
