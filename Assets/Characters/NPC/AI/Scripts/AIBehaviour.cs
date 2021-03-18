using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus
{
    public abstract class AIBehaviour : MonoBehaviour
    {
        protected PlayerController player;

        public virtual void Init(PlayerController player)
        {
            this.player = player;
        }

        /// <summary>
        /// Checks whether the condition of this behaviour is met or not
        /// </summary>
        /// <returns>Whether the behaviour should be continue to carry on its update</returns>
        public abstract bool CheckCondition();
        public abstract void OnUpdate();
    }
}