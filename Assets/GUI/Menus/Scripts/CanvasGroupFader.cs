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
    public UnityEvent onEnable;
    public UnityEvent OnFade;
    public UnityEvent OnFadeEnd;
    public UnityEvent OnFadeUndo;
    public UnityEvent OnFadeUndoEnd;

    private void OnEnable()
    {
        if (UndoFadeOnEnable)
            FadeUndo();
        onEnable.Invoke();
    }

    public void Fade() {
        Fade(duration);
    }

    public void Fade(float _duration)
    {
        var canvGroup = GetComponent<CanvasGroup>();
        canvGroup.DOComplete();
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
        canvGroup.DOComplete();
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
