using BobJeltes.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Pathfinding : MonoBehaviour
{
    private new Rigidbody rigidbody;
    public Rigidbody Rigidbody
    {
        get
        {
            if (rigidbody == null)
            {
                rigidbody = GetComponent<Rigidbody>();
            }
            return rigidbody;
        }
    }

    public bool WarnOnlyOnce = true;
    private bool warningGiven = false;
    [SerializeField] protected WaypointCollection waypointManager;
    public WaypointCollection WaypointManager
    {
        get
        {
            if (!warningGiven && waypointManager == null)
            {
                Debug.LogWarning("No waypoint manager assigned to " + name, this);
                if (WarnOnlyOnce) warningGiven = true;
            }
            else if (!WarnOnlyOnce) warningGiven = false;
            return waypointManager;
        }
        set
        {
            waypointManager = value;
        }
    }

    [Tooltip("When looking for a new waypoint, the pathfinder will first look for the closest waypoint manager in the scene.")]
    public bool SnapToClosestWaypointManager = true;

    public WaypointCollection GetWaypointManager(bool getClosest)
    {
        if (getClosest)
        {
            WaypointManager = WaypointCollectionManager.Instance.GetClosestWaypointCollection(transform.position);
        }
        return WaypointManager;
    }

    private NavMeshAgent navMeshAgent;
    public NavMeshAgent NavMeshAgent
    {
        get
        {
            if (navMeshAgent == null)
                navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            return navMeshAgent;
        }
    }

    public enum Method
    {
        RandomWaypoint,
        OrderedWaypoint,
        ReverseOrderedWaypoint,
        RandomNavMeshPoint
    }
    [Tooltip("This is the method in which a new waypoint is chosen.")]
    public Method method;
    
    [Tooltip("The speed with which the pathfinder moves.")]
    public float speed = 1f;
    [Min(0)]
    public float MaxWalkDistance = 1f;
    
    [Space]
    
    [Tooltip("When moving towards a waypoint, the pathfinder will gradually turn towards the waypoint.")]
    public bool rotateTowardsWaypoint = true;
    [Tooltip("The speed with which the pathfinder will turn towards the waypoint.")]
    public float rotationSpeed = 1f;

    [Space]
    
    [Tooltip("How close the pathfinder has to be to their waypoint before choosing a new waypoint")]
    public float waypointProximity = 1f;
    
    [Tooltip("When enabled, the current waypoint is automatically emptied upon reaching the proximity")]
    public bool useWaypointProximity = true;
    [Tooltip("When enabled, a new waypoint is automatically picked when the current waypoint is set to 'none'")]
    public bool autoPickNewWaypoint = true;
    
    [Space]
    
    [Tooltip("Tip: to have a pathfinder constantly move towards 1 target, put the desired target in this field and set Waypoint Proximity to 0.")]
    public Transform currentWaypoint;
    [SerializeField]
    private Vector3 currentGoal;
    public Vector3 CurrentGoal
    {
        get
        {
            if (currentWaypoint != null)
            {
                currentGoal = currentWaypoint.position;
            }
            return currentGoal;
        }

        set => currentGoal = value;
    }
    private int currentWaypointIndex;


    public Transform goal;
    [Min(0)]
    public float GoalUpdateInterval = .5f;

    public UnityEvent WaypointReached;

    private void Start()
    {
        WaypointCollection wpManager = GetWaypointManager(SnapToClosestWaypointManager);
        if (wpManager == null)
        {
            return;
        }

        if (method == Method.RandomNavMeshPoint)
            StartCoroutine(UpdateGoalPosition(GoalUpdateInterval));
        //currentWaypoint = WaypointManager.GetRandomWaypoint();
    }

    public IEnumerator UpdateGoalPosition(float interval)
    {
        Debug.Log("Update goal position");
        NavMeshAgent.destination = goal.position;
        yield return new WaitForSeconds(interval);
        StartCoroutine(UpdateGoalPosition(interval));
    }

    private void Update()
    {
        if (currentWaypoint == null)
        {
            if (autoPickNewWaypoint)
            {
                WaypointCollection wpManager = GetWaypointManager(SnapToClosestWaypointManager);
                if (wpManager == null)
                    return;
                switch (method)
                {
                    case Method.RandomWaypoint:
                        currentWaypoint = WaypointManager.GetRandomWaypoint();
                        break;
                    case Method.OrderedWaypoint:
                        currentWaypoint = WaypointManager.GetNextWaypoint(currentWaypointIndex, out currentWaypointIndex);
                        break;
                    case Method.ReverseOrderedWaypoint:
                        currentWaypoint = WaypointManager.GetPreviousWaypoint(currentWaypointIndex, out currentWaypointIndex);
                        break;
                    case Method.RandomNavMeshPoint:
                        Vector3 randomDirection = Random.insideUnitCircle * MaxWalkDistance;
                        randomDirection += transform.position;
                        NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, MaxWalkDistance, 1);
                        Vector3 finalPosition = hit.position;
                        NavMeshAgent.destination = finalPosition;
                        break;
                }
            }
        }
        else if (useWaypointProximity && Vector3.Distance(CurrentGoal, transform.position) < waypointProximity)
        {
            currentWaypoint = null;
            WaypointReached.Invoke();
        }
    }

    public float StuckSpeedThreshold = .5f;
    [Tooltip("The amount of time the pathfinder needs to stand still trying to reach a goal, before applying a random force in an attempt to break free")]
    public float unstuckTime = 1f;
    private float unstuckTimeLeft = 1f;
    public float UnstuckForce = 1f;

    public void Unstuck()
    {
        //Debug.Log(Rigidbody.velocity.magnitude);

        // Speed is high, not stuck
        if (Mathf.Abs(Rigidbody.velocity.magnitude) > .5f)
        {
            unstuckTimeLeft = unstuckTime;
            return;
        }

        // Reset time and try get unstuck
        if (unstuckTimeLeft <= 0f)
        {
            unstuckTimeLeft = unstuckTime;

            Rigidbody.AddForce(Extensions.RandomVector301().normalized * UnstuckForce, ForceMode.Impulse);
            return;
        }

        unstuckTimeLeft -= Time.fixedDeltaTime;
    }

    private void FixedUpdate()
    {
        Vector3 heading = (CurrentGoal - Rigidbody.position).normalized;
        Unstuck();

        Rigidbody.AddForce(heading * speed);

        if (rotateTowardsWaypoint)
        {
            Vector3 currentRotation = transform.rotation.eulerAngles;
            Vector3 desiredRotation = Quaternion.LookRotation(heading).eulerAngles;
            Vector3 rotationDelta = desiredRotation - currentRotation;

            ClampToHalfCircles(ref rotationDelta);

            Vector3 torque = rotationDelta * rotationSpeed;

            Rigidbody.AddRelativeTorque(torque);
        }
    }

    /// <summary>
    /// Use this to get the shortest rotation to the desired euler. 
    /// </summary>
    /// <param name="euler"></param>
    public void ClampToHalfCircles(ref Vector3 euler)
    {
        // Limit the angle to 360 degrees
        euler.x %= 360f;
        euler.y %= 360f;
        euler.z %= 360f;

        // If the angle is larger than 180 degrees, reduce it by 360 to make it return the same rotation but within half a circle
        // 359 is larger than 180. 359 - 360 = -1. This results in a 1 degree turn to the left, instead of turning an entire circle to the right.
        if (euler.x > 180f)
        {
            euler.x -= 360f;
        }
        else if (euler.x < -180f)
        {
            euler.x += 360f;
        }

        if (euler.y > 180f)
        {
            euler.y -= 360f;
        }
        else if (euler.y < -180f)
        {
            euler.y += 360f;
        }
        if (euler.z > 180f)
        {
            euler.z -= 360f;
        }
        else if (euler.z < -180f)
        {
            euler.z += 360f;
        }
    }

#if UNITY_EDITOR

    [Header("Debug")]
    public IndicatorType waypointProximityIndicator;
    public enum IndicatorType
    {
        Circle,
        Sphere,
        Off
    }

    public Color indicatorColor = Color.white;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = indicatorColor;
        Handles.color = indicatorColor;
        Gizmos.DrawLine(transform.position, CurrentGoal);
        if (rotateTowardsWaypoint)
        {
            Vector3 direction = CurrentGoal - transform.position;
            if (direction != Vector3.zero)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(direction);
                float dot = Quaternion.Dot(transform.rotation, desiredRotation);
                float angle = Quaternion.Angle(transform.rotation, desiredRotation);
                Vector3 angles = transform.rotation.eulerAngles - desiredRotation.eulerAngles;
                float distance = Vector3.Distance(transform.position, CurrentGoal);

                Vector3 from = transform.forward;
                if (angles.y > 0f && angles.y < 180f)
                {
                    angle *= -1f;
                }

                //Debug.Log("Angles: " + angles);
                Handles.DrawSolidArc(
                    transform.position,
                    transform.up,
                    from,
                    angle,
                    distance);
            }
        }

        switch (waypointProximityIndicator)
        {
            case IndicatorType.Circle:
                Handles.color = indicatorColor;
                Handles.DrawWireDisc(transform.position, transform.up, waypointProximity);
                break;
            case IndicatorType.Sphere:
                Gizmos.DrawWireSphere(transform.position, waypointProximity);
                break;
            case IndicatorType.Off:
                break;
        }

        Gizmos.DrawWireSphere(transform.position, MaxWalkDistance);
    }
#endif
}
