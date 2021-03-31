using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class Succeeder : BTDecorator
    {
        public Succeeder(BehaviourController controller, BTNode child) : base(controller, child)
        {
        }

        public override void Interrupt()
        {
            child.Interrupt();
        }

        public override Result Tick()
        {
            child.Tick();
            return Result.Success;
        }
    }
}