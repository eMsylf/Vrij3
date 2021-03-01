﻿using Boo.Lang;
using RanchyRats.Gyrus;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
//[RequireComponent(typeof(Pathfinding))]
public class EnemyAI : MonoBehaviour
{
    Enemy enemy;

    public FMODUnity.StudioEventEmitter attackSound;
    public FMODUnity.StudioEventEmitter idleSound;
    public void PlaySound(FMODUnity.StudioEventEmitter sound)
    {
        if (sound == null)
            Debug.LogWarning("Sound is not assigned", gameObject);
        else
            sound.Play();
    }
    public void StopSound(FMODUnity.StudioEventEmitter sound)
    {
        if (sound == null)
            Debug.LogWarning("Sound is not assigned", gameObject);
        else
            sound.Stop();
    }

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

    [SerializeField]
    private Pathfinding pathfinding;
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

    public List<PlayerCharacter> targetPlayers;
    public void SortTargetPlayersList()
    {
        bool differenceDetected = false;
        while (differenceDetected)
        {
            differenceDetected = false;
            for (int i = 0; i < targetPlayers.Count - 1; i++)
            {
                PlayerCharacter thisPlayer = targetPlayers[i];
                PlayerCharacter nextPlayer = targetPlayers[i + 1];
                if (DistanceToPlayer(thisPlayer) > DistanceToPlayer(nextPlayer))
                {
                    targetPlayers[i] = nextPlayer;
                    targetPlayers[i + 1] = thisPlayer;
                    differenceDetected = true;
                }
            }
        }
    }
    public PlayerCharacter GetClosestPlayer()
    {
        if (targetPlayers == null || targetPlayers.Count == 0)
            return null;

        return targetPlayers[0];
    }

    public float SightRange = 5f;
    public LayerMask sightObstructions;
    public float AttackRange = 3f;

    [Tooltip("The amount of time the AI will spend in the Idle state, randomly picked between these values. X = min, y = max")]
    public Vector2 IdleTime = new Vector2(1f, 5f);
    
    public enum States
    {
        Idle,
        Wander,
        FollowSingleTarget,
        Attack
    }
    public States state;

    void Update()
    {
        AnyState();
        switch (state)
        {
            case States.Idle:
                Idle();
                break;
            case States.Wander:
                Wander();
                break;
            case States.FollowSingleTarget:
                FollowSingleTarget();
                break;
            case States.Attack:
                Attack();
                break;
        }
    }

    private float DistanceToPlayer(PlayerCharacter player)
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }

    Vector3 playerLastSeenPosition;

    private bool PlayerVisible(PlayerCharacter player, out Vector3 playerPos)
    {
        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = player.transform.position - transform.position;
        Ray ray = new Ray(rayOrigin, rayDirection);
        if (Physics.Raycast(ray, out RaycastHit hit, SightRange, sightObstructions, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("Player view obstructed by " + hit.collider.name + " on layer " + hit.collider.gameObject.layer.ToString(), hit.collider);
            // Use old player position
            playerPos = playerLastSeenPosition;
            return false;
        }
        else
        {
            //Debug.DrawRay(rayOrigin, rayDirection, Color.red, 1f, SightRange);
            // Update player last seen position
            playerLastSeenPosition = player.transform.position;
            playerPos = playerLastSeenPosition;
            Debug.Log("Player in view");
            return true;
        }
    }

    private void OnDisable()
    {
        StopSound(attackSound);
        StopSound(idleSound);
    }

    #region States
    [Tooltip("If the player has been out of range for this duration, the enemy AI will stop following the player")]
    public float playerOutOfRangeCooldown = 3f;
    private float playerOutOfRangeCooldownCurrent;
    private void AnyState()
    {
        SortTargetPlayersList();
        PlayerCharacter closestPlayer = GetClosestPlayer();
        if (closestPlayer == null)
        {
            return;
        }

        float distanceToPlayer = DistanceToPlayer(closestPlayer);
        if (distanceToPlayer < AttackRange)
        {
            ToAttack();
            return;
        }
        if (distanceToPlayer < SightRange)
        {
            ToFollowSingleTarget();
            return;
        }
    }

    private float idleTimeCurrent = 0f;

    public void ToIdle()
    {
        if (state == States.Idle)
        {
            //Debug.Log("Already in idle state");
            return;
        }

        idleTimeCurrent = Random.Range(IdleTime.x, IdleTime.y);
        //Debug.Log("To Idle for " + idleTimeCurrent);
        state = States.Idle;
        Pathfinding.enabled = false;
        if (idleSound == null)
            Debug.LogWarning("Idle sound not assigned", gameObject);
        else
            idleSound.Play();
        
    }
    private void Idle()
    {
        if (idleTimeCurrent > 0f)
        {
            //idleSound.Play();
            idleTimeCurrent -= Time.deltaTime;
        }
        else
        {
            ToWander();
        }
    }

    public void ToWander()
    {
        state = States.Wander;
        Pathfinding.enabled = true;
        Pathfinding.autoPickNewWaypoint = true;
        Pathfinding.useWaypointProximity = true;
    }
    private void Wander()
    {
    }

    public void ToFollowSingleTarget()
    {
        if (state == States.FollowSingleTarget)
        {
            //Debug.Log("Already following single target");
            return;
        }

        //Debug.Log("Transition to Follow Single Target");
        state = States.FollowSingleTarget;
        Pathfinding.enabled = true;
        Pathfinding.autoPickNewWaypoint = false;
        Pathfinding.currentWaypoint = null;
        Pathfinding.useWaypointProximity = false;
    }

    private void FollowSingleTarget()
    {
        PlayerCharacter closestPlayer = GetClosestPlayer();
        if (closestPlayer == null)
            return;

        if (!closestPlayer.enabled)
        {
            ToIdle();
            return;
        }

        if (DistanceToPlayer(closestPlayer) < SightRange)
        {
            Debug.Log("Player is within sight range");
            if (PlayerVisible(closestPlayer, out Vector3 playerPos))
            {
                Pathfinding.CurrentGoal = playerPos;
                return;
            }
            //Pathfinding.CurrentGoal = UpdateLatestPlayerSightedPosition();
            return;
        }

        // Player out of sight
        if (playerOutOfRangeCooldownCurrent <= 0f)
        {
            playerOutOfRangeCooldownCurrent = playerOutOfRangeCooldown;
            Pathfinding.currentWaypoint = null;
            ToIdle();
            return;
        }

        playerOutOfRangeCooldownCurrent -= Time.deltaTime;
    }

    public void ToAttack()
    {
        if (state == States.Attack)
        {
            //Debug.Log("Already in attack state");
            return;
        }
        //Debug.Log("Transition to Attack");
        state = States.Attack;
        Pathfinding.enabled = false;
        attackSound.Play();
    }
    private void Attack()
    {
        PlayerCharacter closestPlayer = GetClosestPlayer();
        if (closestPlayer == null)
            return;

        if (closestPlayer.enabled)
            ToIdle();
    }
    #endregion

#if UNITY_EDITOR
    public Color TargetingColor = Color.yellow;
    public Color AttackColor = Color.red;
    private void OnDrawGizmosSelected()
    {
        Handles.color = TargetingColor;
        Handles.DrawWireDisc(transform.position, transform.up, SightRange);
        Handles.color = AttackColor;
        Handles.DrawWireDisc(transform.position, transform.up, AttackRange);
    }
#endif
}
