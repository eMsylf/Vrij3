using System.Collections.Generic;
using UnityEngine;
using RanchyRats.Gyrus;

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
#pragma warning disable CS0649
    [SerializeField]
    private AIAttitude aIAttitude;
#pragma warning restore 
    public AIAttitude GetAttitude()
    {
        if (aIAttitude == null) Debug.LogError("AI attitude is not assigned");
        return aIAttitude;
    }

    public LayerMask AdditionalAggressionLayers = new LayerMask();

    [Header("States")]
    public States state;
    public enum States
    {
        Idle,
        Wander,
        FollowSingleTarget,
        Attack
    }
    public AIState idle = new AIState(new Vector2(0f, 5f), -1f, 0f);
    public AIState alert = new AIState(new Vector2(0f, 5f), -1f, 25f);
    public AIState searching = new AIState(new Vector2(0f, 5f), 5f, 25f);
    public AIState aggressive = new AIState(new Vector2(0f, 5f), -3f, 50f);

    public AIState GetState(States state)
    {
        switch (state)
        {
            default:
            case States.Idle:
                return idle;
            case States.Wander:
                return alert;
            case States.FollowSingleTarget:
                return searching;
            case States.Attack:
                return aggressive;
        }
    }

    [Header("Targeting")]
    public List<Character> targetCharacters = new List<Character>();
    public Cone SightCone = new Cone();
    [Tooltip("If the player has been out of range for this duration, the enemy AI will stop following the player")]
    public float OutOfRangeTimeout = 3f;

    public bool StopWhileAttacking = true;

    private void OnDisable()
    {
        idle.startSound.Stop();
    }

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

    public void SortTargetPlayersList()
    {
        bool differenceDetected = false;
        while (differenceDetected)
        {
            differenceDetected = false;
            for (int i = 0; i < targetCharacters.Count - 1; i++)
            {
                Character thisCharacter = targetCharacters[i];
                Character nextCharacter = targetCharacters[i + 1];
                if (DistanceToCharacter(thisCharacter) > DistanceToCharacter(nextCharacter))
                {
                    // Switch the characters positions
                    targetCharacters[i] = nextCharacter;
                    targetCharacters[i + 1] = thisCharacter;
                    differenceDetected = true;
                }
            }
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
        if (distanceToCharacter < aggressive.VisionRangeModifier)
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

        idleTimeRemaining = Random.Range(idle.TimeLimit.x, idle.TimeLimit.y);
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
            aggressionCooldown = OutOfRangeTimeout;
            pathfinding.NavMeshAgent.isStopped = true;
            ToIdle();
            return;
        }

        aggressionCooldown -= Time.deltaTime;
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
    private void OnDrawGizmosSelected()
    {
        AIState aiState = GetState(state);

        SightCone.Draw(transform.position, transform.forward, transform.up, aiState.VisionRangeModifier, aiState.FieldOfViewModifier);

        UnityEditor.Handles.color = SightCone.handleColor;
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, SightCone.radius + aiState.VisionRangeModifier);

        // Misschien: Laat alle ranges zien. De niet-actieve ranges zijn vervaagd
    }
#endif
}
