using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class GetObjectsFromRange : Range
    {
        public LayerMask layers;

        public GetObjectsFromRange(BehaviourController controller, float range) : base(controller, range)
        {
        }

        public override void Interrupt()
        {
        }

        public override Result Tick()
        {
            Physics.OverlapSphere(controller.transform.position, range, layers, QueryTriggerInteraction.UseGlobal);
            return Result.Success;
        }
    }
}