using System.Collections;
using UnityEngine;
using BobJeltes.StandardUtilities;

public class TimeManager : Singleton<TimeManager>
{
    public float slowmotionDuration = 10f;
    public float slowmotionTimeScale = .05f;
    public float SlowmotionTimeScale
    {
        get => slowmotionTimeScale;
        set => slowmotionTimeScale = value;
    }
    public float slowmotionRemaining = 10f;
    [Range(0.0001f, 1f)]
    public float timeScale = 1f;
    [Space]
    public float slowmotionTransitionTime = 1f;
    public AnimationCurve SlowmotionTransition = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

    private float fixedScaledDeltaTime => originalFixedDeltaTime * timeScale;
    private float originalFixedDeltaTime;
    private bool slowmotionActive;
    private bool transitioning = false;

    private void Awake()
    {
        SetTimeScale(1f);
        originalFixedDeltaTime = Time.fixedDeltaTime;
    }

    private void Update()
    {
        if (slowmotionActive)
        {
            if (slowmotionRemaining > 0f)
            {
                slowmotionRemaining -= Time.unscaledDeltaTime;
            }
            else
                StopSlowmotion();
        }
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (UnityEditor.EditorApplication.isPlaying)
            SetTimeScale(timeScale);
    }
#endif
    public void SetTimeScale(float timeScale)
    {
        this.timeScale = timeScale;
        Time.timeScale = this.timeScale;
        if (originalFixedDeltaTime == 0f) originalFixedDeltaTime = Time.fixedDeltaTime;
        Time.fixedDeltaTime = fixedScaledDeltaTime;
    }

    private void ResetTimeScale()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixedDeltaTime;
    }

    [ContextMenu("Start timed slowmotion")] public void StartSlowmotion() => StartSlowmotionWithDuration(slowmotionTimeScale, slowmotionDuration);
    public void StartSlowmotion(float factor) => StartSlowmotionWithDuration(factor, slowmotionDuration);
    public void StartSlowmotionWithDuration(float factor, float duration)
    {
        //Debug.Log(factor + " slowmo for " + duration + " seconds.");
        SetTimeScale(factor);
        slowmotionActive = true;
        slowmotionRemaining = duration;
    }

    [ContextMenu("Stop slowmotion")]
    public void StopSlowmotion()
    {
        ResetTimeScale();
        if (transitioning)
        {
            StopAllCoroutines();
            transitioning = false;
        }
        slowmotionActive = false;
    }

    [ContextMenu("Transition into slowmotion")] public void TransitionIntoSlowmotion() => StartCoroutine(TransitionToSlowmotion(slowmotionTransitionTime, slowmotionTimeScale));
    public IEnumerator TransitionToSlowmotion(float transitionTime, float factor)
    {
        if (transitioning)
            yield break;

        transitioning = true;
        float newTimeScale = factor;
        float transitionTimeRemaining = transitionTime;
        
        while (transitionTimeRemaining > 0f)
        {
            float progress = Mathf.InverseLerp(transitionTime, 0f, transitionTimeRemaining);
            yield return new WaitForEndOfFrame();
            float CurveEvaluation = SlowmotionTransition.Evaluate(progress);
            timeScale = Mathf.Lerp(1f, newTimeScale, CurveEvaluation);
            SetTimeScale(timeScale);
            transitionTimeRemaining -= Time.unscaledDeltaTime;
        }
        transitioning = false;
        StartSlowmotion(factor);
    }
}
