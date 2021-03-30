using RanchyRats.Gyrus.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallel : BTComposite
{
    public Parallel(BehaviourController controller, params BTNode[] nodes) : base(controller, nodes)
    {
        throw new System.NotImplementedException();
    }

    public override Result Tick()
    {
        // Wanneer stopt deze?
        // Eén van de children returnt een fail
        // "Do while"
        throw new System.NotImplementedException();
    }
}
