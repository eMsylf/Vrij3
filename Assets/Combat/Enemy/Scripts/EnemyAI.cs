using Combat;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
//[RequireComponent(typeof(Pathfinding))]
public class EnemyAI : MonoBehaviour
{
    Enemy enemy;

    public FMODUnity.StudioEventEmitter attackSound;
    public FMODUnity.StudioEventEmitter idleSound;

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

    public float SightRange = 5f;
    public LayerMask sightObstructions;
    public float AttackRange = 3f;

    [Tooltip("The amount of time the AI will spend in the Idle state, randomly picked between these values. X = min, y = max")]
    public Vector2 IdleTime = new Vector2(1f, 5f);
    
    //public Vector2 time = new MinMaxSlider(1f, 10f, 0f, 60f).value;

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

    private float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
    }

    Vector3 playerLastSeenPosition;

    private bool PlayerVisible(out Vector3 playerPos)
    {
        Vector3 rayOrigin = transform.position;
        Vector3 rayDirection = PlayerController.Instance.transform.position - transform.position;
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
            playerLastSeenPosition = PlayerController.Instance.transform.position;
            playerPos = playerLastSeenPosition;
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
        float distanceToPlayer = DistanceToPlayer();
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
        if (!PlayerController.Instance.enabled)
        {
            ToIdle();
            return;
        }

        if (DistanceToPlayer() < SightRange)
        {
            Debug.Log("Player is within sight range");
            if (PlayerVisible(out Vector3 playerPos))
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
    }
    private void Attack()
    {
        attackSound.Play();
        if (!PlayerController.Instance.enabled)
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
