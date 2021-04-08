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

    [Header("Boss animation")]
    public Animator animator;

    #region Sound
    //[Header("Boss Sounds")]
    //public FMODUnity.StudioEventEmitter IdleSound;
    //public FMODUnity.StudioEventEmitter ScreamSound;
    //public FMODUnity.StudioEventEmitter EyePopSound;
    //public FMODUnity.StudioEventEmitter TeethClackSound;
    //public FMODUnity.StudioEventEmitter AttAnnounceSound;

    void PlayScreamAttackSound() { /*IdleSound.Stop(); ScreamSound.Play();*/}
    void PlayEyePopSound() {/* IdleSound.Stop(); EyePopSound.Play();*/ }
    void PlayTeethClackSound() { /*TeethClackSound.Play();*/}
    void PlayAttAnnounceSound() { /*IdleSound.Stop(); AttAnnounceSound.Play();*/ }
    void PlayDeathSound() { /*IdleSound.Stop(); dieSound.Play();*/}
    void PLayIdleSound() { /*IdleSound.Play();*/ }

    #endregion

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
