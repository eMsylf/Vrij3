using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

    [SerializeField] protected WaypointManager waypointManager;
    public WaypointManager WaypointManager
    {
        get
        {
            if (waypointManager == null)
            {
                Debug.LogError("No waypoint manager assigned to " + name, this);
            }
            return waypointManager;
        }
    }

    public enum Method
    {
        RandomWaypoint,
        OrderedWaypoint,
        ReverseOrderedWaypoint
    }
    public Method method;
    
    public float speed;
    public float waypointProximity;
    public bool rotateTowardsWaypoint;
    public float rotationSpeed;

    public Transform currentWaypoint;
    public int currentWaypointIndex;


    private void Start()
    {
        if (WaypointManager == null)
            return;
        currentWaypoint = WaypointManager.GetRandomWaypoint();
    }

    private void Update()
    {
        if (currentWaypoint == null)
        {
            if (WaypointManager == null)
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

        if (Vector3.Distance(currentWaypoint.position, transform.position) < waypointProximity)
        {
            currentWaypoint = null;
        }
    }

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
            Vector3 currentEuler = transform.rotation.eulerAngles;
            Quaternion desiredRotation = Quaternion.LookRotation(heading);
            Vector3 desiredEuler = desiredRotation.eulerAngles;
            Vector3 rotationForce = desiredEuler - currentEuler;
            rotationForce *= rotationSpeed;
            Rigidbody.AddRelativeTorque(rotationForce);
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
