using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class Sequence : BTComposite
    {
        public override Result Tick()
        {
            for (; i < nodes.Length; i++)
            {
                var result = nodes[i].Tick();
                switch (result)
                {
                    case Result.Failure:
                        i = 0;
                        return result;
                    case Result.Running:
                        return result;
                }
            }
            i = 0;
            return Result.Success;
        }
    }
}