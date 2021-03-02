using RanchyRats.Gyrus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [Header("Boss")]
    public GameObjectEmitter ScreamEmitter;
    public Spawner eyeSpawner;
    public Spawner motmugSpawner;

    [System.Serializable]
    public struct BossSounds
    {
        public FMODUnity.StudioEventEmitter IdleSound;
        public FMODUnity.StudioEventEmitter ScreamSound;
        public FMODUnity.StudioEventEmitter EyePopSound;
        public FMODUnity.StudioEventEmitter TeethClackSound;
        public FMODUnity.StudioEventEmitter AttAnnounceSound;
    }
    public BossSounds bossSounds;
    
    // TODO: Dit kan misschien ook met events gedaan worden. Vergroot modulariteit, hoef je niet de code in te duiken wanneer er een nieuwe sound toegevoegd moet worden.
    void PlayScreamAttackSound() { bossSounds.IdleSound.Stop(); bossSounds.ScreamSound.Play(); }
    void PlayEyePopSound() { bossSounds.IdleSound.Stop(); bossSounds.EyePopSound.Play(); }
    void PlayTeethClackSound() { bossSounds.TeethClackSound.Play(); }
    void PlayAttAnnounceSound() { bossSounds.IdleSound.Stop(); bossSounds.AttAnnounceSound.Play(); }
    void PlayDeathSound() { bossSounds.IdleSound.Stop(); sounds.death.Play(); }
    void PLayIdleSound() { bossSounds.IdleSound.Play(); }

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
        if (Animator == null)
        {
            Debug.LogError("Animator not assigned", gameObject);
            return;
        }

        Animator.SetTrigger("Death");
    }

    public void PlayerClose(bool isClose)
    {
        if (Animator == null)
        {
            Debug.LogError("Animator not assigned", gameObject);
            return;
        }

        Animator.SetBool("PlayerClose", isClose);
    }
}
