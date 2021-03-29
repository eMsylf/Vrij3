using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class GetObjectsFromRadius : Range
    {
        public LayerMask layers;

        public override void Init(PlayerController player)
        {
            base.Init(player);
        }

        /// <summary>
        /// Always succeeds
        /// </summary>
        /// <returns>True</returns>
        public override bool CheckCondition()
        {
            return true;
        }

        /// <summary>
        /// Checks a sphere at the position of the transform, with the specified range, on the specified layers
        /// </summary>
        public override void OnUpdate()
        {
            // Waar laat ik deze lijst met objecten?
            Physics.OverlapSphere(transform.position, range, layers, QueryTriggerInteraction.UseGlobal);
        }

        public override Result Tick()
        {
            // Succeedt deze altijd?
            return Result.Success;
        }
    }
}