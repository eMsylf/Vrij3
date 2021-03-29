using RanchyRats.Gyrus.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screature : BehaviourController
{
    public float SightRange = 10f;
    public LayerMask layers = new LayerMask();
    public FMODUnity.StudioEventEmitter scream;
    public GameObject screamPrefab;

    protected override void Start()
    {
        //tree = new Sequence(
        //    this,
        //    new BTDebug("Started"),
        //    new Wait(1f),
        //    new BTDebug("Ended"),
        //    new Wait(.5f)
        //    );

        tree = new Sequence(this,
            new Wait(1f),
            new BTDebug("start"),
            new CheckObjectsInRange(this, SightRange, layers),
            new BTDebug("objects in range"),
            new PlaySound(this, scream),
            new SpawnObject(this, screamPrefab, true)
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
