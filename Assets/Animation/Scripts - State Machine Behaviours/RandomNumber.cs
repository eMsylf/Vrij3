using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNumber : StateMachineBehaviour
{
    [Header("Enter")]
    public bool RollOnEnter;
    [Tooltip("A random number is picked between 0 and Max-1")]
    public int enterMax;
    [Tooltip("The parameter you'd like the random number to be applied to.")]
    public string EnterAnimatorParameter;

    [Header("Exit")]
    public bool RollOnExit;
    [Tooltip("A random number is picked between 0 and Max-1")]
    public int exitMax;
    [Tooltip("The parameter you'd like the random number to be applied to.")]
    public string ExitAnimatorParameter;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!RollOnEnter)
            return;
        Debug.Log("Enter state: " + stateInfo.ToString());
        int number = Random.Range(0, exitMax);
        Debug.Log("Random number picked: " + number);
        animator.SetInteger(EnterAnimatorParameter, number);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!RollOnExit)
            return;
        Debug.Log("Exit state: " + stateInfo.ToString());
        int number = Random.Range(0, exitMax);
        Debug.Log("Random number picked: " + number);
        animator.SetInteger(ExitAnimatorParameter, number);
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
}
