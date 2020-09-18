﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TimeManager : MonoBehaviour
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

    private void Awake()
    {
        Debug.Log("hi this is the awake");
        Controls.Game.Enable();
        Controls.Game.Attack.performed += _ => DoSlowmotion(slowdownFactor);
        Controls.Game.Dodge.performed += _ => StopSlowmotion();
    }

    private float savedTimeScale;
    private float savedFixedDeltaTime;
    public void DoSlowmotion(float factor)
    {
        Debug.Log("Slowmo");

        savedTimeScale = Time.timeScale;
        savedFixedDeltaTime = Time.fixedDeltaTime;
        Time.timeScale = factor;
        
        Time.fixedDeltaTime = Time.timeScale * savedFixedDeltaTime;
    }

    public void StopSlowmotion()
    {
        Debug.Log("Unslomo");
        Time.timeScale = savedTimeScale;
        Time.fixedDeltaTime = savedFixedDeltaTime;
    }

    public IEnumerator DoSlowmotion(float transitionTime, float factor)
    {
        savedTimeScale = Time.timeScale;
        float newTimeScale = savedTimeScale * factor;
        savedFixedDeltaTime = Time.fixedDeltaTime;
        float newFixedDeltaTime = Time.timeScale * savedFixedDeltaTime;

        bool transitionComplete = false;
        float deltaTime = 0f;
        while (!transitionComplete)
        {
            Debug.Log("Transitioning...");
            float timeBefore = Time.time;
            yield return new WaitForEndOfFrame();
            float timeAfter = Time.time;
            deltaTime = timeAfter - timeBefore;
            transitionTime -= deltaTime;

            Time.timeScale = Mathf.Lerp(savedTimeScale, savedTimeScale, transitionTime);
        }
    }
}
