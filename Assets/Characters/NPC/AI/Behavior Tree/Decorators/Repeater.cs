using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class Repeater : BTDecorator
    {
        public Repeater(BehaviourController controller, BTNode child) : base(controller, child)
        {
        }

        public override Result Tick()
        {
            child.Tick();
            return Result.Running;
        }
    }
}