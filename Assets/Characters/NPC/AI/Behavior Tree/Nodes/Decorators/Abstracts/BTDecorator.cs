using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public abstract class BTDecorator : BTNode
    {
        public BTNode child;

        protected BTDecorator(BehaviourController controller, BTNode child) : base(controller)
        {
            this.child = child;
        }

        public override void Interrupt()
        {
            child.Interrupt();
        }
    }
}