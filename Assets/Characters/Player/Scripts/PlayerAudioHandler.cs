using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RunicSounds;
using System;
using UnityEngine.InputSystem;

public class PlayerAudioHandler : MonoBehaviour {

    [SerializeField] private PlayerController playerController = default;
    [Space]
    [Header("Audio settings")]
    [SerializeField] private AudioBank audioBankGameplay = default;
    [SerializeField] private AudioEvent audioEventFootstep = default;
    [SerializeField] private AudioEvent audioEventDash = default;
    [SerializeField] private AudioEvent audioEventPlayerAttack = default;
    [SerializeField] private AudioEvent audioEventPlayerAttackv2 = default;
    [SerializeField] private AudioEvent audioEventPlayerAttackv3 = default;

    private AudioEvent currentAttackSound = default;

    private IEnumerator Start() {
        audioBankGameplay.LoadBank();
        while (audioBankGameplay.IsLoadOperationInProgress) {
            yield return null;
        }

        playerController.PlayerDodge += OnPlayerDodge;
        playerController.PlayerAttack += OnPlayerAttack;

        currentAttackSound = audioEventPlayerAttack;
    }

    private void Update() {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) {
            currentAttackSound = audioEventPlayerAttack;
        } else if (Keyboard.current.digit2Key.wasPressedThisFrame) {
            currentAttackSound = audioEventPlayerAttackv2;
        }
        else if (Keyboard.current.digit3Key.wasPressedThisFrame) {
            currentAttackSound = audioEventPlayerAttackv3;
        }
    }

    private void OnDestroy() {
        playerController.PlayerDodge -= OnPlayerDodge;
        playerController.PlayerAttack -= OnPlayerAttack;
    }

    /// <summary>
    /// Plays footstep sound
    /// DO NOT RENAME, CALLED BY ANIMATION EVENT
    /// </summary>
    public void OnPlayerFootstep() {
        audioEventFootstep.PlayOneShot(gameObject);
    }

    private void OnPlayerDodge() {
        audioEventDash.PlayOneShot(gameObject);
    }

    private void OnPlayerAttack(float charge) {
        currentAttackSound.PlayOneShot(gameObject);
    }
}
