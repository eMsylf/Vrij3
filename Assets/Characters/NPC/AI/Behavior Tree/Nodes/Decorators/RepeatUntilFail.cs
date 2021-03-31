using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class RepeatUntilFail : BTDecorator
    {
        public RepeatUntilFail(BehaviourController controller, BTNode child) : base(controller, child)
        {
        }

        public override Result Tick()
        {
            var result = child.Tick();
            switch (result)
            {
                case Result.Running:
                case Result.Success:
                    return Result.Running; // Klopt dit?
                case Result.Failure:
                    return Result.Success;
            }
            return default;
        }
    }
}