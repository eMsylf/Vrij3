using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BobJeltes.Attributes;
using UnityEngine.AI;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class BehaviourController : MonoBehaviour
    {
        public BTNode tree;
        public NavMeshAgent navMeshAgent;
        //public Blackboard blackboard;
        public Animator animator;

        protected virtual void Start()
        {
            animator = GetComponent<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            tree?.Tick();
        }
    }
}