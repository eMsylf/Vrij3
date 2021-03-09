using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public Pathfinding pathfinding;
    public NavMeshAgent navMeshAgent;

    public enum Attitude
    {
        Passive,
        Aggressive
    }
    public Attitude aIType;

    public struct AIState
    {

    }

    public AIState Idle;
    public AIState Searching;
    public AIState Aggressive;
    public AIState Attacking;

    public Transform goal;
    [Min(0)]
    public float GoalUpdateInterval = .5f;
    
    private void Start()
    {
        StartCoroutine(UpdateGoalPosition(GoalUpdateInterval));
    }

    public IEnumerator UpdateGoalPosition(float interval)
    {
        Debug.Log("Update goal position");
        navMeshAgent.destination = goal.position;
        yield return new WaitForSeconds(interval);
        StartCoroutine(UpdateGoalPosition(interval));
    }
}
