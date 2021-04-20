using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chance100 : StateMachineBehaviour
{
    [Range(0, 100)]
    public float[] markers = new float[3];
    [BobJeltes.Attributes.ReadOnly] public float[] stored = new float[3];

    [Header("Visualization")]
    public float[] chances = new float[2];
    public string InfluencedParameter = "";
    public int pickedNumber = 0;

    public Color color1 = Color.black;
    public Color color2 = Color.white;

    private void OnValidate()
    {
        if (stored == null || stored.Length != markers.Length)
        {
            Debug.Log("Rebuilding stored variables");
            stored = new float[markers.Length];
        }

        if (chances == null || chances.Length != markers.Length-1)
        {
            chances = new float[markers.Length - 1];
        }

        for (int i = 0; i < chances.Length; i++)
        {
            chances[i] = markers[i + 1] - markers[i];
        }

        markers[0] = 0;
        stored[0] = 0;
        markers[markers.Length - 1] = 100;
        stored[stored.Length - 1] = 100;

        for (int i = 0; i < markers.Length; i++)
        {
            //Debug.Log("Value of " + i + " is " + chances[i] + " was " + stored[i]);
            if (markers[i] < stored[i])
            {
                //Debug.Log("Lowering number " + i + " to value " + chances[i]);
                // Lowering
                for (int j = i; j > 0; j--)
                {
                    // Compare the number with the previous, if it's not the first number
                    if (markers[j] < markers[j - 1])
                    {
                        // Smaller than previous number
                        markers[j - 1] = markers[j];
                        stored[j - 1] = markers[j - 1];
                    }
                }
                stored[i] = markers[i];
                break;
            }
            else if (markers[i] > stored[i])
            {
                //Debug.Log("Increasing number " + i + " to value " + chances[i]);
                // Increasing
                for (int j = i; j < markers.Length - 1; j++)
                {
                    // Compare the number with the next, if it's not the last number
                    if (markers[j] > markers[j + 1])
                    {
                        // Bigger than next number
                        markers[j + 1] = markers[j];
                        stored[j + 1] = markers[j + 1];
                    }
                }
                stored[i] = markers[i];
                break;
            }
        }
    }

    public bool RollOnEnter = true;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!RollOnEnter)
            return;
        Debug.Log("Enter state: " + stateInfo.ToString());
        float number = UnityEngine.Random.Range(0f, 100f);
        
        animator.SetInteger(InfluencedParameter, GetPickedState(number));
    }

    public int GetPickedState(float number)
    {
        for (int i = 0; i < markers.Length; i++)
        {
            if (number < markers[i])
            {
                pickedNumber = i;
                return pickedNumber;
            }
        }
        return markers.Length - 1;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
