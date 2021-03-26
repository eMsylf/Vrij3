using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class CheckObjectsInRange : Range
    {
        public LayerMask layers;

        public override void Init(PlayerController player)
        {
            base.Init(player);
        }

        /// <summary>
        /// Checks if there are objects within range on the specified layer(s)
        /// </summary>
        /// <returns>Whether there are objects in the sphere on the specified layer(s)</returns>
        public override bool CheckCondition()
        {
            return Physics.CheckSphere(transform.position, range, layers, QueryTriggerInteraction.UseGlobal);
        }

        public override void OnUpdate()
        {
            // Moet hier iets gebeuren?
        }
    }
}