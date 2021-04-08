using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RunicSounds;

public class ScreatureAudioHandler : MonoBehaviour
{
    [SerializeField] private AudioEvent audioEventAttacking = default;
    [SerializeField] private AudioEvent audioEventIsHit = default;

    public void AttackStart()
    {
        audioEventAttacking.Play(gameObject);
    }

    public void AttackEnd()
    {
        audioEventAttacking.Stop();
    }

    public void IsHitStart()
    {
        audioEventIsHit.PlayOneShot(gameObject);
    }
}
