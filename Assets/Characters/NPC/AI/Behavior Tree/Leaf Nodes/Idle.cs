using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class Idle : Action
    {
        public override bool CheckCondition()
        {
            return true;
        }

        public override void OnUpdate()
        {
            Debug.Log("Idle");
        }
    }
}