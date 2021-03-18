using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RanchyRats.Gyrus
{
    public class BehaviourController : MonoBehaviour
    {
        private List<AIBehaviour> behaviours;
        private AIBehaviour currentBehaviour;

        private void Start()
        {
            GetComponents(behaviours);
        }

        public void OnUpdate()
        {
            currentBehaviour?.OnUpdate();
        }

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