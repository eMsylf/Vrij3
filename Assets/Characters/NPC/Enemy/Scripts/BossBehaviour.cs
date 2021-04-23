using RanchyRats.Gyrus.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : BehaviourController
{
    [Space]
    public GameObjectEmitter ScreamEmitter;
    public Spawner eyeSpawner;
    public Spawner motmugSpawner;

    [Min(0)]
    public float
        VisionCheckInterval = 1f,
        SightRange = 10f,
        AttackCooldown = 3f;
    public LayerMask dangerLayers = new LayerMask();

    protected override void Start()
    {
        tree =
            new Sequence(this,
                new CheckObjectsInRange(this, SightRange, dangerLayers),
                new SetAnimatorParameter(this, "Attack"),
                new RandomSelector(this,
                    new SetAnimatorParameter(this, "ScreamAttack"),
                    new SetAnimatorParameter(this, "EyeAttack"),
                    new SetAnimatorParameter(this, "MotmugAttack")
                ),
                new Wait(AttackCooldown)
            )
            ;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, SightRange);
    }
}
