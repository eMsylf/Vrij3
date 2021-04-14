using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    [System.Serializable]
    public abstract class BTNode
    {
        internal BehaviourController controller;

        public BTNode(BehaviourController controller)
        {
            this.controller = controller;
        }

        public abstract Result Tick();
        public abstract void Interrupt();
    }
}