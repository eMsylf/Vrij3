using BobJeltes.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTrueFalse : StateMachineBehaviour
{
    [Tooltip("A number between 0 and 100. \nLower = more chance at FALSE. \nHigher = more chance at TRUE.")]
    [Range(0, 100)]
    public int Weight = 50;
    public enum Timing
    {
        Enter,
        StartOfState,
        Exit,
    }
    public Timing timing = Timing.Enter;
    public enum ParameterType
    {
        Bool,
        Trigger
    }
    public ParameterType parameterType = ParameterType.Bool;
    public string AnimatorParameter;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeInState = 0f;

        if (timing == Timing.Enter)
        {
            RollTrueFalse(animator);
        }
    }

    private float timeInState = 0f;

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (timing == Timing.StartOfState)
        {
            if (timeInState < stateInfo.length)
            {
                timeInState += Time.deltaTime;
                return;
            }
            timeInState = 0f;
            RollTrueFalse(animator);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (timing == Timing.Exit)
        {
            RollTrueFalse(animator);
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

    public void RollTrueFalse(Animator animator)
    {
        bool outcome = Extensions.RandomTrueFalse(Weight);
        switch (parameterType)
        {
            case ParameterType.Bool:
                animator.SetBool(AnimatorParameter, outcome);
                break;
            case ParameterType.Trigger:
                if (outcome)
                    animator.SetTrigger(AnimatorParameter);
                break;
            default:
                break;
        }
    }
}
