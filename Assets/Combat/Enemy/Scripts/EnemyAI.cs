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

    public float TargetingProximity = 5f;
    public float AttackingProximity = 3f;

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

    private float CheckPlayerDistance()
    {
        return Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
    }

    #region States
    [Tooltip("If the player has been out of range for this duration, the enemy AI will stop following")]
    public float playerOutOfRangeCooldown = 3f;
    private float playerOutOfRangeCooldownCurrent;
    private void AnyState()
    {
        float playerDistance = CheckPlayerDistance();
        if (playerDistance < AttackingProximity)
        {
            ToAttack();
            return;
        }
        if (playerDistance < TargetingProximity)
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
        Pathfinding.currentWaypoint = PlayerController.Instance.transform;
        Pathfinding.useWaypointProximity = false;
    }
    private void FollowSingleTarget()
    {
        if (!PlayerController.Instance.enabled)
        {
            ToIdle();
            return;
        }

        if (CheckPlayerDistance() < TargetingProximity)
            return;

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
        Handles.DrawWireDisc(transform.position, transform.up, TargetingProximity);
        Handles.color = AttackColor;
        Handles.DrawWireDisc(transform.position, transform.up, AttackingProximity);
    }
#endif
}
