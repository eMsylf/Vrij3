using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        currentWaypoint = WaypointManager.GetRandomWaypoint();
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Rigidbody.AddForce((transform.position - currentWaypoint.position) * speed);
    }
}
