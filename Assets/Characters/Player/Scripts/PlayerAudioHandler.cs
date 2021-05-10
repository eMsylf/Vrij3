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

    public void HandlePlayerAttack(float charge)
    {
        audioEventPlayerAttack.PlayOneShot(gameObject, null)
            .SetParameter(audioParameterPlayerAttackCharge, charge)
            .Release(); ;
    }
}
