using RanchyRats.Gyrus;
using RanchyRats.Gyrus.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : BehaviourController
{
    [Header("Boss")]
    public GameObjectEmitter ScreamEmitter;
    public Spawner eyeSpawner;
    public Spawner motmugSpawner;
    [Min(0)]
    public float 
        VisionCheckInterval = 1f, 
        SightRange = 10f;
    public LayerMask layers = new LayerMask();

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
    //void PlayDeathSound() { bossSounds.IdleSound.Stop(); Death.sound.Play(); }
    void PLayIdleSound() { bossSounds.IdleSound.Play(); }

    protected override void Start()
    {
        tree =
            new Selector(this,
                new Sequence(this,
                    new CheckObjectsInRange(this, SightRange, layers)
                    //new Selector
                    )
            );
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

    //public override void Die()
    //{
    //    base.Die();
    //    StartDeathAnimation();
    //}

    //public void StartDeathAnimation()
    //{
    //    if (Animator == null)
    //    {
    //        Debug.LogError("Animator not assigned", gameObject);
    //        return;
    //    }

    //    Animator.SetTrigger("Death");
    //}

    //public void PlayerClose(bool isClose)
    //{
    //    if (Animator == null)
    //    {
    //        Debug.LogError("Animator not assigned", gameObject);
    //        return;
    //    }

    //    Animator.SetBool("PlayerClose", isClose);
    //}
}
