using RanchyRats.Gyrus.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallel : BTComposite
{
    public Parallel(BehaviourController controller, params BTNode[] nodes) : base(controller, nodes)
    {
        
    }

    public override Result Tick()
    {
        // Wanneer stopt deze?
        // "Do while"
        throw new System.NotImplementedException();
    }
}
