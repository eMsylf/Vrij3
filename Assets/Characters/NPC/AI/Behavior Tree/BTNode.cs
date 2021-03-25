using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public abstract class BTNode : MonoBehaviour
    {
        /// <summary>
        /// Checks whether the condition of this behaviour is met or not
        /// </summary>
        /// <returns>Whether the behaviour should be continued on its update</returns>
        public abstract bool CheckCondition();
        public abstract void OnUpdate();
    }
}