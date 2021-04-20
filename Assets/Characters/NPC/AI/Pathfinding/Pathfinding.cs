using BobJeltes.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace RanchyRats.Gyrus
{
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
        public bool FindsClosestWaypointManager = true;

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

        [Header("Moving")]

        [Tooltip("The speed with which the pathfinder moves.")]
        public float speed = 1f;
        [Min(0)]
        public float MaxWalkDistance = 1f;

        [Header("Turning")]

        [Tooltip("When moving towards a waypoint, the pathfinder will gradually turn towards the waypoint.")]
        public bool turnTowardsWaypoint = true;
        [Tooltip("The speed with which the pathfinder will turn towards the waypoint.")]
        public float rotationSpeed = 1f;

        [Header("Stopping")]

        [Tooltip("How close the pathfinder has to be to their goal before stopping")]
        public float stoppingDistance = 1f;

        [Tooltip("When enabled, the current goal is automatically set to null upon reaching the stopping distance")]
        public bool clearGoal = true;
        [Tooltip("When enabled, a new waypoint is automatically picked when the current waypoint is set to 'none'")]
        public bool autoPickNewGoal = true;

        [Space]

        [Tooltip("Tip: to have a pathfinder constantly move towards 1 target, put the desired target in this field and set Waypoint Proximity to 0.")]
        public Transform goal;
        [SerializeField]
        private Vector3 destination;
        public Vector3 Destination
        {
            get
            {
                if (goal != null)
                {
                    destination = goal.position;
                }
                return destination;
            }

            set => destination = value;
        }
        private int currentWaypointIndex;

        [Min(0)]
        public float GoalUpdateInterval = .5f;

        public UnityEvent OnGoalReached;

        [Header("Unstuck")]
        public float StuckSpeedThreshold = .5f;
        [Tooltip("The amount of time the pathfinder needs to stand still trying to reach a goal, before applying a random force in an attempt to break free")]
        public float unstuckTime = 1f;
        private float unstuckTimeLeft = 1f;
        public float UnstuckForce = 1f;

        private void Start()
        {
            WaypointCollection wpManager = GetWaypointManager(FindsClosestWaypointManager);
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
            if (goal != null)
            {
                if (clearGoal && Vector3.Distance(Destination, transform.position) < stoppingDistance)
                {
                    goal = null;
                    OnGoalReached.Invoke();
                }

                return;
            }

            if (autoPickNewGoal)
            {
                WaypointCollection wpManager = GetWaypointManager(FindsClosestWaypointManager);
                if (wpManager == null)
                    return;
                switch (method)
                {
                    case Method.RandomWaypoint:
                        goal = WaypointManager.GetRandomWaypoint();
                        break;
                    case Method.OrderedWaypoint:
                        goal = WaypointManager.GetNextWaypoint(currentWaypointIndex, out currentWaypointIndex);
                        break;
                    case Method.ReverseOrderedWaypoint:
                        goal = WaypointManager.GetPreviousWaypoint(currentWaypointIndex, out currentWaypointIndex);
                        break;
                    case Method.RandomNavMeshPoint:
                        // TODO: Test implementation
                        Vector3 randomDirection = Random.insideUnitCircle * MaxWalkDistance;
                        randomDirection += transform.position;
                        NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, MaxWalkDistance, 1);
                        Vector3 finalPosition = hit.position;
                        NavMeshAgent.destination = finalPosition;
                        break;
                }
            }
        }

        private void FixedUpdate()
        {
            switch (method)
            {
                default:
                case Method.RandomWaypoint:
                case Method.OrderedWaypoint:
                case Method.ReverseOrderedWaypoint:
                    NavMeshAgent.enabled = false;
                    BuiltInPathfindingMovement();
                    break;
                case Method.RandomNavMeshPoint:
                    NavMeshAgent.enabled = true;
                    // Movement regulated by the navmesh agent
                    break;
            }
        }

        public void BuiltInPathfindingMovement()
        {
            Vector3 heading = (Destination - Rigidbody.position).normalized;
            Unstuck();

            Rigidbody.AddForce(heading * speed);

            if (turnTowardsWaypoint)
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
        /// Use this to get the shortest rotation to the desired euler angles. 
        /// </summary>
        /// <param name="eulerAngles">The angles in degrees</param>
        public void ClampToHalfCircles(ref Vector3 eulerAngles)
        {
            ClampToHalfCircle(ref eulerAngles.x);
            ClampToHalfCircle(ref eulerAngles.y);
            ClampToHalfCircle(ref eulerAngles.z);
        }

        public void ClampToHalfCircle(ref float angle)
        {
            // Limit the angle to 360 degrees
            angle %= 360f;
            // If the angle is larger than 180 degrees, reduce it by 360 to make it return the same rotation but within half a circle
            // EXAMPLE: 359 is larger than 180. 359 - 360 = -1. This results in a 1 degree turn to the left, instead of turning almost an entire circle (359 degrees) to the right.
            if (angle > 180f)
            {
                angle -= 360f;
            }
            else if (angle < -180f)
            {
                angle += 360f;
            }
        }

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
            Gizmos.DrawLine(transform.position, Destination);
            if (turnTowardsWaypoint)
            {
                Vector3 direction = Destination - transform.position;
                if (direction != Vector3.zero)
                {
                    Quaternion desiredRotation = Quaternion.LookRotation(direction);
                    float dot = Quaternion.Dot(transform.rotation, desiredRotation);
                    float angle = Quaternion.Angle(transform.rotation, desiredRotation);
                    Vector3 angles = transform.rotation.eulerAngles - desiredRotation.eulerAngles;
                    float distance = Vector3.Distance(transform.position, Destination);

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
                    Handles.DrawWireDisc(transform.position, transform.up, stoppingDistance);
                    break;
                case IndicatorType.Sphere:
                    Gizmos.DrawWireSphere(transform.position, stoppingDistance);
                    break;
                case IndicatorType.Off:
                    break;
            }

            Gizmos.DrawWireSphere(transform.position, MaxWalkDistance);
        }
#endif
    }
}