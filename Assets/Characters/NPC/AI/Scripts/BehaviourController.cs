using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BobJeltes.Attributes;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class BehaviourController : MonoBehaviour
    {
        private List<Action> actions = new List<Action>();
        private Action currentAction;

        private void Start()
        {
            GetComponents(actions);
        }

        public void OnUpdate()
        {
            currentAction?.OnUpdate();
        }

        /// <summary>
        /// Re-evaluates all behaviours in order, and sets the current behaviour to whichever meets the condition to run.
        /// </summary>
        public void CheckForNewBehaviour()
        {
            // Wanneer check je voor een nieuwe behavior?
            foreach (var behaviour in actions)
            {
                if (behaviour.CheckCondition())
                {
                    currentAction = behaviour;
                    break;
                }
            }
        }
    }
}