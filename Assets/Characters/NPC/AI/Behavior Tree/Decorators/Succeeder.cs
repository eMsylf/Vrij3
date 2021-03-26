using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class Succeeder : BTDecorator
    {
        public override Result Tick()
        {
            return Result.Success;
        }
    }
}