using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BobJeltes.Attributes;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class BehaviourController : MonoBehaviour
    {
        private List<Action> behaviours = new List<Action>();
        private Action currentBehaviour;

        private void Start()
        {
            GetComponents(behaviours);
        }

        public void OnUpdate()
        {
            currentBehaviour?.OnUpdate();
        }

        /// <summary>
        /// Re-evaluates all behaviours in order, and sets the current behaviour to whichever meets the condition to run.
        /// </summary>
        public void CheckForNewBehaviour()
        {
            // Wanneer check je voor een nieuwe behavior?
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