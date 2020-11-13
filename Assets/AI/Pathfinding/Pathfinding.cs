using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
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

    [SerializeField] protected WaypointCollection waypointManager;
    public WaypointCollection WaypointManager
    {
        get
        {
            if (waypointManager == null)
            {
                Debug.LogError("No waypoint manager assigned to " + name, this);
            }
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

    public enum Method
    {
        RandomWaypoint,
        OrderedWaypoint,
        ReverseOrderedWaypoint
    }
    [Tooltip("This is the method in which a new waypoint is chosen.")]
    public Method method;
    
    [Tooltip("The speed with which the pathfinder moves.")]
    public float speed = 1f;
    
    [Space]
    
    [Tooltip("When moving towards a waypoint, the pathfinder will gradually turn towards the waypoint.")]
    public bool rotateTowardsWaypoint = true;
    [Tooltip("The speed with which the pathfinder will turn towards the waypoint.")]
    public float rotationSpeed = 1f;

    [Space]
    
    [Tooltip("How close the pathfinder has to be to their waypoint before choosing a new waypoint")]
    public float waypointProximity = 1f;
    public bool useWaypointProximity = true;
    
    [Space]
    
    [Tooltip("Tip: to have a pathfinder constantly move towards 1 target, put the desired target in this field and set Waypoint Proximity to 0.")]
    public Transform currentWaypoint;
    private int currentWaypointIndex;


    private void Start()
    {
        WaypointCollection wpManager = GetWaypointManager(SnapToClosestWaypointManager);
        if (wpManager == null)
        {
            return;
        }
        //currentWaypoint = WaypointManager.GetRandomWaypoint();
    }

    private void Update()
    {
        if (currentWaypoint == null)
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
            }
        }
        else if (useWaypointProximity && Vector3.Distance(currentWaypoint.position, transform.position) < waypointProximity)
        {
            currentWaypoint = null;
            WaypointReached.Invoke();
        }
    }

    public UnityEvent WaypointReached;

    private void FixedUpdate()
    {
        if (currentWaypoint == null)
        {
            return;
        }

        Vector3 heading = currentWaypoint.position - Rigidbody.position;

        Rigidbody.AddForce(heading * speed);

        if (rotateTowardsWaypoint)
        {
            Quaternion currentRotation = transform.rotation;
            Quaternion desiredRotation = Quaternion.LookRotation(heading);
            Vector3 currentEuler = transform.rotation.eulerAngles;
            Vector3 desiredEuler = desiredRotation.eulerAngles;
            Vector3 eulerDifference = desiredEuler - currentEuler;

            ClampToHalfCircles(ref eulerDifference);

            Vector3 torque = eulerDifference * rotationSpeed;

            Rigidbody.AddRelativeTorque(torque);
            //Debug.Log("Current euler: " + currentEuler + " Desired euler: " + desiredEuler + " difference: " + eulerDifference + " Multiplied difference (torque): " + torque);
        }
    }

    public void ClampToHalfCircles(ref Vector3 euler)
    {
        euler.x %= 360f;
        euler.y %= 360f;
        euler.z %= 360f;

        if (euler.x > 180f)
        {
            euler.x -= 360f;
        }
        if (euler.x < -180f)
        {
            euler.x += 360f;
        }
        if (euler.y > 180f)
        {
            euler.y -= 360f;
        }
        if (euler.y < -180f)
        {
            euler.y += 360f;
        }
        if (euler.z > 180f)
        {
            euler.z -= 360f;
        }
        if (euler.z < -180f)
        {
            euler.z += 360f;
        }
    }

#if UNITY_EDITOR
    public enum IndicatorType
    {
        Circle,
        Sphere,
        Off
    }
    [Header("Debug")]
    public IndicatorType waypointProximityIndicator;
    public Color indicatorColor = Color.white;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = indicatorColor;
        Handles.color = indicatorColor;
        if (currentWaypoint != null)
        {
            Gizmos.DrawLine(transform.position, currentWaypoint.position);
            if (rotateTowardsWaypoint)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(currentWaypoint.position - transform.position);
                float dot = Quaternion.Dot(transform.rotation, desiredRotation);
                float angle = Quaternion.Angle(transform.rotation, desiredRotation);
                Vector3 angles = transform.rotation.eulerAngles - desiredRotation.eulerAngles;
                float anglesMag = angles.magnitude;


                Vector3 heading = currentWaypoint.position - transform.position;

                Vector3 from = transform.forward;
                if (angles.y > 0f && angles.y < 180f)
                {
                    angle *= -1f;
                }

                //Debug.Log("Angles: " + angles);
                //Debug.Log("Angles magnitude: " + anglesMag);
                //Debug.Log("Dot: " + dot);
                Handles.DrawSolidArc(
                    transform.position, 
                    transform.up, 
                    from, 
                    angle, 
                    Vector3.Distance(transform.position, currentWaypoint.position));
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
    }
#endif
}
