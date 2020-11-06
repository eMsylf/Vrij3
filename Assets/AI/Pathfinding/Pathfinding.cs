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
        Rigidbody.AddForce((currentWaypoint.position - transform.position) * speed);
    }

#if UNITY_EDITOR
    [Header("Debug")]
    public bool sphereIndicator;
    public Color indicatorColor = Color.white;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = indicatorColor;
        if (currentWaypoint != null)
        {
            Gizmos.DrawLine(transform.position, currentWaypoint.position);
        }
        if (sphereIndicator)
            Gizmos.DrawWireSphere(transform.position, waypointProximity);
        else
        {
            Handles.color = indicatorColor;
            Handles.DrawWireDisc(transform.position, transform.up, waypointProximity);
        }
    }
#endif
}
