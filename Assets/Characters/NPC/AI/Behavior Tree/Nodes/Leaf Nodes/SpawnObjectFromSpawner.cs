using RanchyRats.Gyrus.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectFromSpawner : BTNode
{
    Spawner spawner;
    public SpawnObjectFromSpawner(BehaviourController controller, Spawner spawner) : base(controller)
    {
        this.spawner = spawner;
    }

    public override void Interrupt()
    {
        throw new System.NotImplementedException();
    }

    public override Result Tick()
    {
        if (spawner == null) return Result.Failure;
        spawner.Spawn();
        return Result.Success;
    }
}
