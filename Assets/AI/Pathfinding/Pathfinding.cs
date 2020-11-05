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

    public enum Method
    {
        Waypoints
    }
    public Method method;

    public float speed;

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

    public Transform currentWaypoint;
    public int currentWaypointIndex;

    public float waypointProximity;
    public bool autoGetRandomWaypoint;

    private void Start()
    {
        currentWaypoint = WaypointManager.GetRandomWaypoint();
    }

    private void Update()
    {
        if (currentWaypoint == null)
        {
            currentWaypoint = WaypointManager.GetRandomWaypoint();
            return;
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
    private void OnDrawGizmosSelected()
    {
        if (currentWaypoint != null)
        {
            Gizmos.DrawLine(transform.position, currentWaypoint.position);
        }
        Handles.DrawWireDisc(transform.position, transform.up, waypointProximity);
    }
#endif
}
