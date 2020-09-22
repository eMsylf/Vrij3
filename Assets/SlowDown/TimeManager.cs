using System.Collections;
using UnityEngine;
using BobJeltes.StandardUtilities;

public class TimeManager : Singleton<TimeManager>
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

    public float slowdownFactor = .05f;
    public float slowdownLength = 2f;
    public float slowdownTransition = 1f;
    public AnimationCurve SlowdownTransition = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
    public bool DebugButtonsEnabled = false;

    private float slowmoTime = 99f;
    private float savedTimeScale;
    private float savedFixedDeltaTime;
    private bool slowmo = false;
    private bool transitioning = false;

    private void Awake()
    {
        if (DebugButtonsEnabled)
        {
            Controls.Game.Enable();
            Controls.Game.Attack.performed += _ => DoSlowmotion(slowdownFactor);
            Controls.Game.Dodge.performed += _ => StopSlowmotion();
            Controls.Game.LockOn.performed += _ => StartCoroutine(TransitionToSlowmotion(slowdownTransition, slowdownFactor));
        }
    }

    private void Update()
    {
        if (slowmoTime < slowdownLength)
        {
            slowmo = true;
            Time.timeScale = slowdownFactor;
            slowmoTime += Time.unscaledDeltaTime;
            slowmoTime = Mathf.Clamp(slowmoTime + Time.deltaTime, 0f, slowdownLength);
            if (slowmoTime >= slowdownLength)
            {
                Time.timeScale = savedTimeScale;
                Time.fixedDeltaTime = savedFixedDeltaTime;
                slowmo = false;
                Debug.Log("Stop slowmo");
            }
        }
    }

    public void OverrideSavedTimeScale()
    {
        Debug.Log("Saved timescale " + Time.timeScale);
        savedTimeScale = Time.timeScale;
        savedFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void DoSlowmotion(float factor)
    {
        if (slowmo)
        {
            Debug.Log("Slowmo attempted while already active");
            return;
        }

        slowmo = true;
        Debug.Log("Slowmo");
        if (savedTimeScale == 0f || savedFixedDeltaTime == 0f)
        {
            OverrideSavedTimeScale();
        }
        Time.timeScale = factor;
        Time.fixedDeltaTime = Time.timeScale * savedFixedDeltaTime;
    }

    public void DoSlowmotionWithDuration(float factor, float duration)
    {
        if (slowmo)
        {
            Debug.Log("Slowmo attempted while already active");
            return;
        }
        Debug.Log("Slowmo for " + duration + " seconds");
        
        if (savedTimeScale == 0f || savedFixedDeltaTime == 0f)
        {
            OverrideSavedTimeScale();
        }
        slowdownFactor = factor;
        slowmoTime = 0f;
        slowdownLength = duration;

        slowmo = true;
    }

    public void StopSlowmotion()
    {
        if (!slowmo)
        {
            Debug.Log("Slowmo already stopped");
            return;
        }
        slowmo = false;

        Debug.Log("Unslomo");
        Time.timeScale = savedTimeScale;
        Time.fixedDeltaTime = savedFixedDeltaTime;
        if (transitioning)
        {
            Debug.Log("Transition cancelled");
            StopAllCoroutines();
            transitioning = false;
        }
    }

    public IEnumerator TransitionToSlowmotion(float transitionTime, float factor)
    {
        if (slowmo || transitioning)
            yield break;
        transitioning = true;
        slowmo = true;
        float newTimeScale = savedTimeScale * factor;
        float newFixedDeltaTime = newTimeScale * savedFixedDeltaTime;

        float startTime = Time.time;
        float endTime = startTime + transitionTime;
        while (Time.time < endTime)
        {
            float progress = (Time.time - startTime)/transitionTime;
            yield return new WaitForEndOfFrame();
            float CurveEvaluation = SlowdownTransition.Evaluate(progress);
            Time.timeScale = Mathf.Lerp(savedTimeScale, newTimeScale, CurveEvaluation);
            Time.fixedDeltaTime = Mathf.Lerp(savedFixedDeltaTime, newFixedDeltaTime, CurveEvaluation);


            //Debug.Log("Transitioning... Progress: " + progress + ". Current timeScale: " + Time.timeScale + ". Current FixedDeltaTime: " + Time.fixedDeltaTime);
        }
        //Debug.Log("Done!");
        transitioning = false;
    }
}
