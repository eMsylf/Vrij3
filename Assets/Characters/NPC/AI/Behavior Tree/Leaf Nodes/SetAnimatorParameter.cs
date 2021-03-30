using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class SetAnimatorParameter : BTNode
    {
        private Animator animator;

        private enum ParameterType
        {
            Float,
            Int,
            Bool,
            Trigger
        }
        private ParameterType parameterType;

        private string animatorParameter;
        private int layer;

        // Reference variables
        private float referenceFloat;
        private int referenceInt;
        private bool referenceBool;

        public SetAnimatorParameter(BehaviourController controller, string intName, int value, int layer = 0) : base(controller)
        {
            animator = controller.animator;
            animatorParameter = intName;
            referenceInt = value;
            parameterType = ParameterType.Int;
            this.layer = layer;
        }

        public SetAnimatorParameter(BehaviourController controller, string parameterName, float value, int layer = 0) : base(controller)
        {
            animator = controller.animator;
            animatorParameter = parameterName;
            referenceFloat = value;
            parameterType = ParameterType.Float;
            this.layer = layer;
        }

        public SetAnimatorParameter(BehaviourController controller, string parameterName, bool value, int layer = 0) : base(controller)
        {
            animator = controller.animator;
            animatorParameter = parameterName;
            referenceBool = value;
            parameterType = ParameterType.Bool;
            this.layer = layer;
        }

        public SetAnimatorParameter(BehaviourController controller, string triggerName, int layer = 0) : base(controller)
        {
            animator = controller.animator;
            animatorParameter = triggerName;
            parameterType = ParameterType.Trigger;
            this.layer = layer;
        }

        public override void Interrupt() { }

        public override Result Tick()
        {
            if (animator == null)
            {
                return Result.Failure;
            }

            switch (parameterType)
            {
                case ParameterType.Float:
                    controller.animator.SetFloat(animatorParameter, referenceFloat);
                    break;
                case ParameterType.Int:
                    controller.animator.SetInteger(animatorParameter, referenceInt);
                    break;
                case ParameterType.Bool:
                    controller.animator.SetBool(animatorParameter, referenceBool);
                    break;
                case ParameterType.Trigger:
                    controller.animator.SetTrigger(animatorParameter);
                    break;
                default:
                    break;
            }

            return Result.Success;
        }
    }
}