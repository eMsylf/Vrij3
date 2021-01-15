using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [Header("Boss")]
    public GameObjectEmitter ScreamEmitter;
    public Spawner eyeSpawner;
    public Spawner motmugSpawner;

    public FMODUnity.StudioEventEmitter IdleSound;
    public FMODUnity.StudioEventEmitter ScreamSound;
    public FMODUnity.StudioEventEmitter EyespawnSound;




    [Header("Boss animation")]
    public Animator animator;

    void PlayScreamAttackSound() {
        ScreamSound.Play();
    }

    void PlayDeathSound() {
        dieSound.Play();
    }

    void SpawnScream()
    {
        ScreamEmitter.Emit();
    }

    void SpawnEye()
    {
        eyeSpawner.Spawn();
    }

    void SpawnMotmugs()
    {
        motmugSpawner.Spawn();
    }

    public override void Die()
    {
        base.Die();
        StartDeathAnimation();
    }

    public void StartDeathAnimation()
    {
        if (animator == null)
        {
            Debug.LogError("Animator not assigned", gameObject);
            return;
        }

        animator.SetTrigger("Death");
    }

    public void PlayerClose(bool isClose)
    {
        if (animator == null)
        {
            Debug.LogError("Animator not assigned", gameObject);
            return;
        }

        animator.SetBool("PlayerClose", isClose);
    }
}
