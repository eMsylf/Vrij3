using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class SetAnimatorParameter : BTNode
    {
        private Animator animator;
        private AnimatorControllerParameterType parameterType;

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
            parameterType = AnimatorControllerParameterType.Int;
            this.layer = layer;
        }

        public SetAnimatorParameter(BehaviourController controller, string parameterName, float value, int layer = 0) : base(controller)
        {
            animator = controller.animator;
            animatorParameter = parameterName;
            referenceFloat = value;
            parameterType = AnimatorControllerParameterType.Float;
            this.layer = layer;
        }

        public SetAnimatorParameter(BehaviourController controller, string parameterName, bool value, int layer = 0) : base(controller)
        {
            animator = controller.animator;
            animatorParameter = parameterName;
            referenceBool = value;
            parameterType = AnimatorControllerParameterType.Bool;
            this.layer = layer;
        }

        public SetAnimatorParameter(BehaviourController controller, string triggerName, int layer = 0) : base(controller)
        {
            animator = controller.animator;
            animatorParameter = triggerName;
            parameterType = AnimatorControllerParameterType.Trigger;
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
                case AnimatorControllerParameterType.Float:
                    controller.animator.SetFloat(animatorParameter, referenceFloat);
                    break;
                case AnimatorControllerParameterType.Int:
                    controller.animator.SetInteger(animatorParameter, referenceInt);
                    break;
                case AnimatorControllerParameterType.Bool:
                    controller.animator.SetBool(animatorParameter, referenceBool);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    controller.animator.SetTrigger(animatorParameter);
                    break;
                default:
                    break;
            }

            return Result.Success;
        }
    }
}