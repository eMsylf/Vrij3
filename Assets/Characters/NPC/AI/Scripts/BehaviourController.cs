using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BobJeltes.Attributes;

namespace RanchyRats.Gyrus.AI
{
    public class BehaviourController : MonoBehaviour
    {
        public AIAttitude attitude;
        private List<AIBehaviour> behaviours = new List<AIBehaviour>();
        private AIBehaviour currentBehaviour;

        private void Start()
        {
            GetComponents(behaviours);
        }

        public void OnUpdate()
        {
            currentBehaviour?.OnUpdate();
        }

        /// <summary>
        /// Re-evaluates all behaviours in order, and updates the current behaviour to whichever meets the condition to run.
        /// </summary>
        public void CheckForNewBehaviour()
        {
            foreach (var behaviour in behaviours)
            {
                if (behaviour.CheckCondition())
                {
                    currentBehaviour = behaviour;
                    break;
                }
            }
        }
    }
}