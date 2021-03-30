using RanchyRats.Gyrus.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screature : BehaviourController
{
    [Min(0)]
    public float 
        VisionCheckInterval = 1f, 
        SightRange = 10f;
    public LayerMask layers = new LayerMask();
    public FMODUnity.StudioEventEmitter screamSound;
    public GameObject screamObject;
    [Min(0)]
    public float ScreamDuration = 1f;

    protected override void Start()
    {
        tree = new Sequence(this,
            new Wait(VisionCheckInterval),
            new BTDebug("check"),
            new CheckObjectsInRange(this, SightRange, layers),
            new BTDebug("objects are in range"),
            new PlaySound(this, screamSound, false),
            new SetObjectActive(screamObject, true),
            new SetAnimatorParameter(this, "Scream", true),
            new Wait(ScreamDuration),
            new StopSound(this, screamSound),
            new SetAnimatorParameter(this, "Scream", false),
            new SetObjectActive(screamObject, false)
            );
    }

    void Update()
    {
        tree?.Tick();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, SightRange);
    }
}
