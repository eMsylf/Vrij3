using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    [AddComponentMenu("Ranchy Rats/AI/ApproachTarget")]
    public class ScanForObjects : BTLeaf
    {
        public LayerMask layers;
        public float Range;

        public override void Init(PlayerController player)
        {
            base.Init(player);
        }

        public override bool CheckCondition()
        {
            return true;
        }

        public override void OnUpdate()
        {
             Physics.OverlapSphere(transform.position, Range, layers, QueryTriggerInteraction.UseGlobal);
        }
    }
}