using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BobJeltes.Attributes;

namespace RanchyRats.Gyrus.AI
{
    public abstract class AIBehaviour : MonoBehaviour
    {
        protected PlayerController player;

        // Wat moet een AI allemaal kunnen doen in een state?
        public StudioEventEmitter startSound;
        public enum TimeLimit
        {
            None,
            Constant,
            RandomRange
        }
        public TimeLimit timeLimit;
        public bool HasTimeLimit { get => timeLimit != TimeLimit.None; }

        public float GetTimeLimit()
        {
            switch (timeLimit)
            {
                default:
                case TimeLimit.None:
                    return 0;
                case TimeLimit.Constant:
                    return timeLimitConstant;
                case TimeLimit.RandomRange:
                    return UnityEngine.Random.Range(timeLimitRange.x, timeLimitRange.y);
            }
        }
        [SerializeField]
        [ShowIf("timeLimit", false, 1)]
        private float timeLimitConstant = 0f;
        [Tooltip("The amount of time the AI will spend in the Idle state, randomly picked between these values. x = min, y = max")]
        [SerializeField]
        [ShowIf("timeLimit", false, 2)]
        private Vector2 timeLimitRange = new Vector2();
        internal float timeLeft;
        public float VisionRangeModifier;
        [UnityEngine.Range(0f, 360f)]
        public float FieldOfViewModifier;

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