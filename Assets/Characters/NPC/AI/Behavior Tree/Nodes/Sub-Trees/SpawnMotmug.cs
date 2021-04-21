using RanchyRats.Gyrus.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMotmug : Sequence
{
    public SpawnMotmug(BehaviourController controller, Spawner spawner) : base(controller)
    {
        nodes = new BTNode[] { 
            //new SetObjectActive(spawner, true),
            new SetAnimatorParameter(controller, "Attack"),
            new SetAnimatorParameter(controller, "MotmugAttack"),
            //new SetObjectActive(spawner, false)
        };
    }
}
