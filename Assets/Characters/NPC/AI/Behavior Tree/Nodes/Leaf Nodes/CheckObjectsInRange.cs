using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class CheckObjectsInRange : Range
    {
        public LayerMask layers;

        public CheckObjectsInRange(BehaviourController controller, float range, LayerMask layers) : base(controller, range)
        {
            this.layers = layers;
        }

        public override void Interrupt()
        {

        }

        public override Result Tick()
        {
            if (Physics.CheckSphere(controller.transform.position, range, layers, QueryTriggerInteraction.UseGlobal))
            {
                return Result.Success;
            }
            return Result.Failure;
        }
    }
}