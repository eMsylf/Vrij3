using RanchyRats.Gyrus.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEye : Sequence
{
    public SpawnEye(BehaviourController controller, Spawner spawner) : base(controller)
    {
        nodes = new BTNode[]
        {
            new SpawnObjectFromSpawner(controller, spawner),
            new SetAnimatorParameter(controller, "Attack"),
            new SetAnimatorParameter(controller, "EyeAttack"),
            //new SetObjectActive(spawner, false)
        };
    }
}
