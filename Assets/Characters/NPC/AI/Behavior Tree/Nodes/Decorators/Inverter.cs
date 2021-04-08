using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class Inverter : BTDecorator
    {
        public Inverter(BehaviourController controller, BTNode child) : base(controller, child)
        {
        }

        public override Result Tick()
        {
            var result = child.Tick();
            switch (result)
            {
                case Result.Success:
                    return Result.Failure;
                case Result.Failure:
                    return Result.Success;
                case Result.Running:
                    return Result.Running; // Hoort dit wel iets te doen bij success?
            }
            return default;
        }
    }
}