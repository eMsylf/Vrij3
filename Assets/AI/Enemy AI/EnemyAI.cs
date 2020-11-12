using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(Pathfinding))]
public class EnemyAI : MonoBehaviour
{
    Enemy enemy;
    Enemy Enemy
    {
        get
        {
            if (enemy == null)
            {
                enemy = GetComponent<Enemy>();
            }
            return enemy;
        }
    }

    Pathfinding pathfinding;
    Pathfinding Pathfinding
    {
        get
        {
            if (pathfinding == null)
            {
                pathfinding = GetComponent<Pathfinding>();
            }
            return pathfinding;
        }
    }

    public enum States
    {
        Idle,
        Wander,
        FollowSingleTarget,
        Attack
    }
    public States state;

    void Start()
    {

    }

    void Update()
    {
        
    }
}
