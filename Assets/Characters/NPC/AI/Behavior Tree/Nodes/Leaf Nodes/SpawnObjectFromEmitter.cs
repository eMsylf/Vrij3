using RanchyRats.Gyrus.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectFromEmitter : BTNode
{
    GameObjectEmitter emitter;
    public SpawnObjectFromEmitter(BehaviourController controller, GameObjectEmitter emitter) : base(controller)
    {
        this.emitter = emitter;
    }

    public override void Interrupt()
    {
        throw new System.NotImplementedException();
    }

    public override Result Tick()
    {
        emitter.Emit();
        if (emitter == null) return Result.Failure;
        return Result.Success;
    }
}
