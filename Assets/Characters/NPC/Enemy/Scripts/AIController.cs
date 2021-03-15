using RanchyRats.Gyrus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(RanchyRats.Gyrus.CharacterController))]
public partial class AIController : MonoBehaviour
{
    public Pathfinding pathfinding;

    private RanchyRats.Gyrus.CharacterController characterController;
    public RanchyRats.Gyrus.CharacterController CharacterController
    {
        get
        {
            if (characterController == null)
                GetComponent<RanchyRats.Gyrus.CharacterController>();
            return characterController;
        }
        private set => characterController = value;
    }

    public enum Attitude
    {
        Passive,
        Neutral,
        Hostile
    }
    public Attitude attitude;

    [System.Serializable]
    public struct AIAttitude
    {
        public LayerMask AggressionLayers;
    }

    [SerializeField]
    private AIAttitude
        Passive = new AIAttitude(),
        Neutral = new AIAttitude(),
        Hostile = new AIAttitude();

    public AIAttitude GetAttitude()
    {
        switch (attitude)
        {
            default:
            case Attitude.Passive:
                return Passive;
            case Attitude.Neutral:
                return Neutral;
            case Attitude.Hostile:
                return Hostile;
                //return new AIAttitude();
        }
    }

    [System.Serializable]
    public struct AIState
    {
        // Wat moet een AI allemaal kunnen doen in een state?
        public FMODUnity.StudioEventEmitter startSound;
    }

    public AIState idle;
    public AIState alert;
    public AIState searching;
    public AIState aggressive;

    private void OnDisable()
    {
        idle.startSound.Stop();
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

    public Cone SightCone = new Cone();
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

    private bool IsCharacterVisible(Character character)
    {
        Ray ray = new Ray(
            transform.position,
            character.transform.position - transform.position);

        if (Physics.Raycast(ray, out RaycastHit hit, SightCone.radius, SightCone.layermMask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("Player view obstructed by " + hit.collider.name + " on layer " + hit.collider.gameObject.layer.ToString(), hit.collider);
            // Use old player position
            return false;
        }
        return true;
    }

    #region State machine
    [Tooltip("If the player has been out of range for this duration, the enemy AI will stop following the player")]
    public float characterOutOfRangeCooldown = 3f;
    private float aggressionCooldown;
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
        if (distanceToCharacter < SightCone.radius)
        {
            ToFollowSingleTarget();
            return;
        }
    }

    private float idleTimeRemaining = 0f;

    public void ToIdle()
    {
        if (state == States.Idle)
        {
            //Debug.Log("Already in idle state");
            return;
        }

        idleTimeRemaining = Random.Range(IdleTime.x, IdleTime.y);
        //Debug.Log("To Idle for " + idleTimeCurrent);
        state = States.Idle;
        pathfinding.NavMeshAgent.isStopped = true;
        if (idle.startSound == null)
            Debug.LogWarning("Idle sound not assigned", gameObject);
        else
            idle.startSound.Play();
    }
    private void Idle()
    {
        if (idleTimeRemaining > 0f)
        {
            //idleSound.Play();
            idleTimeRemaining -= Time.deltaTime;
        }
        else
        {
            ToWander();
        }
    }

    public void ToWander()
    {
        state = States.Wander;
        pathfinding.NavMeshAgent.isStopped = false;
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
        pathfinding.NavMeshAgent.isStopped = false;
        // TODO: Reimplement
        //Pathfinding.autoPickNewWaypoint = false;
        //Pathfinding.currentWaypoint = null;
        //Pathfinding.useWaypointProximity = false;
    }

    private void FollowSingleTarget()
    {
        Character closestCharacter = GetClosestCharacter();
        if (closestCharacter == null)
            return;

        if (!closestCharacter.enabled)
        {
            ToIdle();
            return;
        }

        if (DistanceToCharacter(closestCharacter) < SightCone.radius)
        {
            Debug.Log("Player is within sight range");
            if (IsCharacterVisible(closestCharacter))
            {
                pathfinding.NavMeshAgent.destination = closestCharacter.transform.position;
                return;
            }
            //Pathfinding.CurrentGoal = UpdateLatestPlayerSightedPosition();
            return;
        }

        // Player out of sight
        if (aggressionCooldown <= 0f)
        {
            aggressionCooldown = characterOutOfRangeCooldown;
            pathfinding.NavMeshAgent.isStopped = true;
            ToIdle();
            return;
        }

        aggressionCooldown -= Time.deltaTime;
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
        if (StopWhileAttacking) pathfinding.NavMeshAgent.isStopped = false;
        CharacterController.attacking?.sounds.PlayAttackSound(0);
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
    public Color AttackColor = Color.red;
    private void OnDrawGizmosSelected()
    {
        SightCone.Draw(transform.position, transform.forward, transform.up);

        UnityEditor.Handles.color = SightCone.handleColor;
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, SightCone.radius);
        UnityEditor.Handles.color = AttackColor;
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, AttackRange);
    }
#endif
}
