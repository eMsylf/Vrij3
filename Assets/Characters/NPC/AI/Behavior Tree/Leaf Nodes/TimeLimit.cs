using BobJeltes.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class TimeLimit : Action
    {
        public enum type
        {
            Constant,
            RandomRange
        }
        [SerializeField]
        private type timeLimit = type.Constant;

        public float GetTimeLimit()
        {
            switch (timeLimit)
            {
                default:
                case type.Constant:
                    return timeLimitConstant;
                case type.RandomRange:
                    return Random.Range(timeLimitRange.x, timeLimitRange.y);
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

        public TimeLimit(BehaviourController controller, float time) : base(controller)
        {
            timeLimitConstant = time;
        }

        public override Result Tick()
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                return Result.Failure;
            }
            return Result.Running;
        }

        public override void Interrupt()
        {
            throw new System.NotImplementedException();
        }
    }
}