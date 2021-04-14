using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class Selector : BTComposite
    {
        public Selector(BehaviourController controller, params BTNode[] nodes) : base(controller, nodes)
        {
        }

        public override Result Tick()
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                Result result = nodes[i].Tick();
                switch (result)
                {
                    case Result.Success:
                        this.i = 0;
                        return result;
                    case Result.Running:
                        return result;
                }
            }
            i = 0;
            return Result.Failure;
        }
    }
}