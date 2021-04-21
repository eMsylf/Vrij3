using RanchyRats.Gyrus.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : BehaviourController
{
    public GameObjectEmitter ScreamEmitter;
    public Spawner eyeSpawner;
    public Spawner motmugSpawner;

    [Min(0)]
    public float
        VisionCheckInterval = 1f,
        SightRange = 10f;
    public float AttackCooldown = 3f;
    public LayerMask dangerLayers = new LayerMask();
    public Gradient gradient = new Gradient();
    protected override void Start()
    {
        tree =
            new Selector(this,
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
            );
    }
}
