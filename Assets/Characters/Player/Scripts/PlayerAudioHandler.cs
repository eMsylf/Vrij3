using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RunicSounds;
using System;
using UnityEngine.InputSystem;
using RanchyRats.Gyrus;

public class PlayerAudioHandler : MonoBehaviour {

    [Header("Audio settings")]
    [SerializeField] private AudioBank audioBankGameplay = default;
    [SerializeField] private AudioEvent audioEventFootstep = default;
    [SerializeField] private AudioEvent audioEventDash = default;
    [SerializeField] private AudioEvent audioEventPlayerAttack = default;
    [SerializeField] private AudioParameter audioParameterPlayerAttackCharge = default;
    [SerializeField] private AudioEvent audioEventPlayerCharge = default;
    [SerializeField] private AudioParameter audioParameterChargeProgress = default;

    private float lastChargeValue = 0;

    private IEnumerator Start()
    {
        audioBankGameplay.LoadBank();
        while (audioBankGameplay.IsLoadOperationInProgress)
        {
            yield return null;
        }

        //playerController.PlayerDodge += OnPlayerDodge;
        //playerController.PlayerAttack += OnPlayerAttack;
    }

    private void OnDestroy()
    {
        //playerController.PlayerDodge -= OnPlayerDodge;
        //playerController.PlayerAttack -= OnPlayerAttack;
    }

    /// <summary>
    /// Plays footstep sound
    /// DO NOT RENAME, CALLED BY ANIMATION EVENT
    /// </summary>
    public void OnPlayerFootstep()
    {
        audioEventFootstep.PlayOneShot(gameObject, null).Release();
    }

    private void OnPlayerDodge()
    {
        audioEventDash.PlayOneShot(gameObject, null).Release();
    }

    public void HandlePlayerAttack()
    {
        int chargeIntValue = 0;
        if (lastChargeValue != 0)
        {
            chargeIntValue = 1;
        }
        if (lastChargeValue > 0.5f)
        {
            chargeIntValue = 2;
        }
        audioEventPlayerAttack.PlayOneShot(gameObject, null)
            .SetParameter(audioParameterPlayerAttackCharge, chargeIntValue)
            .Release(); ;

        lastChargeValue = 0;
    }

    public void HandleChargeStart()
    {
        audioEventPlayerCharge.Play(gameObject, null);
    }

    public void HandleChargeChanged(float value)
    {
        Debug.Log(value);
        audioEventPlayerCharge.SetParameter(audioParameterChargeProgress, value);
        lastChargeValue = value;
    }

    public void HandleChargeEnd()
    {
        audioEventPlayerCharge.Stop();
    }
}
