using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RunicSounds;

public class ScreatureAudioHandler : MonoBehaviour
{
    [SerializeField] private AudioEvent audioEventAttacking = default;
    [SerializeField] private AudioParameter audioParameterAttackProgress = default;
    [SerializeField] private float attackDuration = 0.75f;
    [SerializeField] private AudioEvent audioEventIsHit = default;

    private float timeAttacking = 0;

    private void Update()
    {
        if (audioEventAttacking.IsPlaying)
        {
            timeAttacking += Time.deltaTime;
            audioEventAttacking.SetParameter(audioParameterAttackProgress, Mathf.Clamp01(timeAttacking / attackDuration));
        }
    }

    public void AttackStart()
    {
        audioEventAttacking.Play(gameObject, null);
        timeAttacking = 0;
    }

    public void AttackEnd()
    {
        audioEventAttacking.Stop();
    }

    public void IsHitStart()
    {
        Debug.Log("Hit sound should play");
        audioEventIsHit.PlayOneShot(gameObject, null).Release();
    }
}
