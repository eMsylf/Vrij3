using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public abstract class Range : Action
    {
        [Min(0)]
        public float range;

        protected Range(BehaviourController controller, float range) : base(controller)
        {
            this.range = range;
        }

        private void OnDrawGizmosSelected()
        {
            //Gizmos.DrawWireSphere(controller.transform.position, range);
        }
    }
}