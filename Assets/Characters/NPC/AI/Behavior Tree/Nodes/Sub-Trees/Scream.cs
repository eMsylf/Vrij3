using RanchyRats.Gyrus.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scream : Sequence
{
    public Scream(BehaviourController controller) : base(controller)
    {
        nodes = new BTNode[]
        {
            new SetAnimatorParameter(controller, "Attack"),
            new SetAnimatorParameter(controller, "ScreamAttack"),
        };
    }
}
