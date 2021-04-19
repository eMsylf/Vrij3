using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;

public class FadeOut : MonoBehaviour
{
    public enum StartFadeType
    {
        None,
        Instant,
        Regular
    }
    [Tooltip("The type of fadeout that is done on start.\n\n" +
        "- None does not fade on start\n" +
        "- Instant can be useful if there are a lot of objects in the scene using a FadeOut component. This setting bypasses DOTween, so the DOTween system does not get flooded with simultaneous tweens.\n" +
        "- Regular fades out using the supplied Delay and Duration")]
    public StartFadeType startFadeType = StartFadeType.Instant;

    public float Delay = 2f;
    public float Duration = 1f;

    private List<float> startAlphaValues;
    private List<float> GetStartAlphaValues()
    {
        if (startAlphaValues == null)
        {
            startAlphaValues = new List<float>();
            foreach (Graphic graphic in GetComponentsInChildren<Graphic>())
            {
                //Debug.Log("Got start alpha value for " + graphic + ": " + graphic.color.a);
                startAlphaValues.Add(graphic.color.a);
            }
        }
        return startAlphaValues;
    }

    private void Awake()
    {
        GetStartAlphaValues();
    }

    void Start()
    {
        switch (startFadeType)
        {
            case StartFadeType.None:
                break;
            case StartFadeType.Instant:
                StartFadeOut(0f, 0f);
                break;
            case StartFadeType.Regular:
                StartFadeOut();
                break;
            default:
                break;
        }
    }

    public UnityEvent OnStartFadeOut = new UnityEvent();
    public UnityEvent OnEndFadeOut = new UnityEvent();

    public void StartFadeOut()
    {
        StartFadeOut(Delay, Duration);
    }

    public void StartFadeOut(float delay, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutRoutine(delay, duration));
    }

    private IEnumerator FadeOutRoutine(float delay, float duration)
    {
        //Debug.Log("Start fade out with delay " + Delay);
        OnStartFadeOut.Invoke();
        yield return new WaitForSeconds(delay);
        Fade(duration);
        yield return new WaitForSeconds(duration);
        OnEndFadeOut.Invoke();
    }

    public void Fade(float duration)
    {
        //Debug.Log("Fade", this);
        if (duration == 0f)
        {
            foreach (Graphic graphic in GetComponentsInChildren<Graphic>())
            {
                Color graphicColor = graphic.color;
                graphicColor.a = 0f;
                //Debug.Log("Fade " + graphic.name + " from " + graphicColor, graphic);
                graphic.color = graphicColor;
            }
        }
        else
        {
            foreach (Graphic graphic in GetComponentsInChildren<Graphic>())
            {
                //Debug.Log("Fade " + graphic.name + " from " + graphic.color, graphic);
                graphic.DOFade(0f, duration);
            }
        }

    }

    public void ResetFade()
    {
        Graphic[] graphics = GetComponentsInChildren<Graphic>();
        for (int i = 0; i < graphics.Length; i++)
        {
            Graphic graphic = graphics[i];
            graphic.DOKill();
            Color resetColor = graphic.color;
            resetColor.a = GetStartAlphaValues()[i];
            graphic.color = resetColor;
        }
    }
}
