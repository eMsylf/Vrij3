using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BobJeltes.Attributes;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public abstract class Action : MonoBehaviour
    {
        protected PlayerController player;

        public virtual void Init(PlayerController player)
        {
            // Wat ga ik hiermee doen? We zoeken nooit direct de player controller op.
            this.player = player;
        }
        /// <summary>
        /// Checks whether the condition of this behaviour is met or not
        /// </summary>
        /// <returns>Whether the behaviour should be continued on its update</returns>
        public abstract bool CheckCondition();
        public abstract void OnUpdate();
    }
}