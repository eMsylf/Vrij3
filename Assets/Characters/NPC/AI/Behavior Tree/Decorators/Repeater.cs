using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class Repeater : BTDecorator
    {
        public override Result Tick()
        {
            return Result.Running; // Hoort dit altijd running terug te sturen?
        }
    }
}