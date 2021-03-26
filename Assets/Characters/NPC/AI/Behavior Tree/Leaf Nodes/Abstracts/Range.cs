using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public abstract class Range : Action
    {
        [Min(0)]
        public float range;

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}