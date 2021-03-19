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

        public bool ShowThings;
        [ShowIf("ShowThings")]
        [UnityEngine.Range(0f, 2f)]
        public float aaaaaaaa;
        [ShowIf("ShowThings")]
        public Vector2 oopsie;
        [ShowIf("ShowThings")]
        public string fug;

        [ReadOnly]
        public Vector3 cantTouchThis;


        public enum TestCondition
        {
            Zero,
            One,
            Two
        }
        public TestCondition testCondition;
        [ShowIf("testCondition", ShowIfAttribute.Type.Enum, 0)]
        public GameObject testObject;
        [ShowIf("testCondition", ShowIfAttribute.Type.Enum, 1)]
        public string testString;
        [ShowIf("testCondition", ShowIfAttribute.Type.Enum, 2)]
        public float testFloat;


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