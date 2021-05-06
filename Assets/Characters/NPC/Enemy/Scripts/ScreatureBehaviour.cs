using RanchyRats.Gyrus.AI.BehaviorTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreatureBehaviour : BehaviourController
{
    [Min(0)]
    public float 
        VisionCheckInterval = 1f, 
        SightRange = 10f;
    public LayerMask layers = new LayerMask();
    public GameObject screamObject;
    [Min(0)]
    public float AnnouncementDuration = 1f,
        ScreamDuration = 1f,
        ScreamInterval = 1f;

    protected override void Start()
    {
        tree =
            new Selector(this,
                new Sequence(this,
                    new CheckObjectsInRange(this, SightRange, layers),
                    new SetAnimatorParameter(this, "Scream", true),
                    new Wait(AnnouncementDuration),
                    new SetObjectActive(screamObject, true),
                    new Wait(ScreamDuration),
                    new SetAnimatorParameter(this, "Scream", false),
                    new SetObjectActive(screamObject, false),
                    new Wait(ScreamInterval)
                ),
                new Idle(VisionCheckInterval)
            );

        SphereCollider collider = new SphereCollider();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, SightRange);
    }
}
