using RanchyRats.Gyrus;
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
        Neutral,
        Hostile
    }
    public Attitude aIType;

    [System.Serializable]
    public struct AIAttitude
    {
        public LayerMask AggressionLayers;

        public AIAttitude(LayerMask aggressionLayers)
        {
            AggressionLayers = aggressionLayers;
        }
    }

    public AIAttitude Passive = new AIAttitude(new LayerMask());
    public AIAttitude Neutral = new AIAttitude();
    public AIAttitude Hostile = new AIAttitude();

    [System.Serializable]
    public struct AIState
    {
        // Wat moet een AI allemaal kunnen doen in een state?
        
    }

    public AIState idle;
    public AIState alert; // ?
    public AIState searching;
    public AIState aggressive;

    public Transform goal;
    [Min(0)]
    public float GoalUpdateInterval = .5f;

    private void OnEnable()
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = true;
        }
    }

    private void OnDisable()
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }
        StopSound(attackSound);
        StopSound(idleSound);
    }

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

    public List<Character> targetCharacters = new List<Character>();

    public void SortTargetPlayersList()
    {
        bool differenceDetected = false;
        while (differenceDetected)
        {
            differenceDetected = false;
            for (int i = 0; i < targetCharacters.Count - 1; i++)
            {
                Character thisPlayer = targetCharacters[i];
                Character nextPlayer = targetCharacters[i + 1];
                if (DistanceToCharacter(thisPlayer) > DistanceToCharacter(nextPlayer))
                {
                    targetCharacters[i] = nextPlayer;
                    targetCharacters[i + 1] = thisPlayer;
                    differenceDetected = true;
                }
            }
        }
    }

    public float SightRange = 5f;
    // TODO: Fix Cone class
    internal Cone SightCone;
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

    public Character GetClosestCharacter()
    {

        if (targetCharacters == null || targetCharacters.Count == 0)
            return null;

        return targetCharacters[0];
    }

    private float DistanceToCharacter(Character character)
    {
        return Vector3.Distance(transform.position, character.transform.position);
    }

    Vector3 playerLastSeenPosition;

    private bool IsCharacterVisible(Character character, out Vector3 characterPosition)
    {
        Ray ray = new Ray(
            transform.position,
            character.transform.position - transform.position);

        if (Physics.Raycast(ray, out RaycastHit hit, SightRange, sightObstructions, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("Player view obstructed by " + hit.collider.name + " on layer " + hit.collider.gameObject.layer.ToString(), hit.collider);
            // Use old player position
            characterPosition = playerLastSeenPosition;
            return false;
        }
        else
        {
            //Debug.DrawRay(rayOrigin, rayDirection, Color.red, 1f, SightRange);
            // Update player last seen position
            playerLastSeenPosition = character.transform.position;
            characterPosition = playerLastSeenPosition;
            Debug.Log("Player in view");
            return true;
        }
    }

    #region States
    [Tooltip("If the player has been out of range for this duration, the enemy AI will stop following the player")]
    public float playerOutOfRangeCooldown = 3f;
    private float playerOutOfRangeCooldownCurrent;
    private void AnyState()
    {
        SortTargetPlayersList();
        Character closestCharacter = GetClosestCharacter();
        if (closestCharacter == null)
        {
            return;
        }

        float distanceToCharacter = DistanceToCharacter(closestCharacter);
        if (distanceToCharacter < AttackRange)
        {
            ToAttack();
            return;
        }
        if (distanceToCharacter < SightRange)
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
        navMeshAgent.isStopped = true;
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
        navMeshAgent.isStopped = false;
        // TODO: Reimplement
        //Pathfinding.autoPickNewWaypoint = true;
        //Pathfinding.useWaypointProximity = true;
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
        navMeshAgent.isStopped = false;
        // TODO: Reimplement
        //Pathfinding.autoPickNewWaypoint = false;
        //Pathfinding.currentWaypoint = null;
        //Pathfinding.useWaypointProximity = false;
    }

    private void FollowSingleTarget()
    {
        Character closestPlayer = GetClosestCharacter();
        if (closestPlayer == null)
            return;

        if (!closestPlayer.enabled)
        {
            ToIdle();
            return;
        }

        if (DistanceToCharacter(closestPlayer) < SightRange)
        {
            Debug.Log("Player is within sight range");
            if (IsCharacterVisible(closestPlayer, out Vector3 playerPos))
            {
                navMeshAgent.destination = playerPos;
                return;
            }
            //Pathfinding.CurrentGoal = UpdateLatestPlayerSightedPosition();
            return;
        }

        // Player out of sight
        if (playerOutOfRangeCooldownCurrent <= 0f)
        {
            playerOutOfRangeCooldownCurrent = playerOutOfRangeCooldown;
            navMeshAgent.isStopped = true;
            ToIdle();
            return;
        }

        playerOutOfRangeCooldownCurrent -= Time.deltaTime;
    }
    public bool StopWhileAttacking = true;
    public void ToAttack()
    {
        if (state == States.Attack)
        {
            //Debug.Log("Already in attack state");
            return;
        }
        //Debug.Log("Transition to Attack");
        state = States.Attack;
        if (StopWhileAttacking) navMeshAgent.isStopped = false;
        attackSound.Play();
    }
    private void Attack()
    {
        Character closestPlayer = GetClosestCharacter();
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
        UnityEditor.Handles.color = TargetingColor;
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, SightRange);
        UnityEditor.Handles.color = AttackColor;
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, AttackRange);
    }
#endif
}
