using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class Idle : BTNode
    {
        BTNode tree;
        public Idle(float timeLimit, BehaviourController controller = null) : base(controller)
        {
            tree =
                    new TimeLimit(timeLimit)
                ;
        }

        public override void Interrupt()
        {
            tree.Interrupt();
        }

        public override Result Tick()
        {
            return tree.Tick();
        }
    }
}