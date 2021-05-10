using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachineBehaviour
{
    [SerializeField] private PlayerStates thisState = default;
    private enum PlayerStates { Idle, Walking, Attack, Hit }
    private PlayerAudioHandler audioHandler = default;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (audioHandler == null)
        {
            audioHandler = animator.GetComponentInParent<PlayerAudioHandler>();
        }

        Debug.Log("ENTER: " + thisState);

        switch (thisState)
        {
            case PlayerStates.Idle:
                break;
            case PlayerStates.Walking:
                break;
            case PlayerStates.Attack:
                audioHandler.HandlePlayerAttack(0);
                break;
            case PlayerStates.Hit:
                break;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("EXIT: " + thisState);
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
