using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    public bool PlayOnStart = true;
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
        if (PlayOnStart)
            StartFadeOut();
    }

    public void StartFadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        //Debug.Log("Start fade out with delay " + Delay);
        yield return new WaitForSeconds(Delay);
        Fade();
    }

    public void Fade()
    {
        //Debug.Log("Fade", this);
        foreach (Graphic graphic in GetComponentsInChildren<Graphic>())
        {
            graphic.DOFade(0f, Duration);
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
