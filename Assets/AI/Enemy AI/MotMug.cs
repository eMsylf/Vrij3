using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotMug : Enemy
{
    Pathfinding pathfinding;
    Pathfinding TryGetPathfinding()
    {
        if (pathfinding == null)
        {
            pathfinding = GetComponent<Pathfinding>();
        }
        return pathfinding;
    }

    public bool movementJitter = true;
    public Vector3 JitterAmount = Vector3.one;

    private void Update()
    {
        ManagePathfinding();
    }

    private void FixedUpdate()
    {
        MovementJitter();
    }

    private void MovementJitter()
    {
        if (!movementJitter)
        {
            return;
        }
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
            return;

        rb.AddForce(RandomVector3(JitterAmount));
    }

    public Vector3 RandomVector3()
    {
        return RandomVector3(1f);
    }

    public Vector3 RandomVector3(float bounds)
    {
        return RandomVector3(bounds, bounds, bounds);
    }

    public Vector3 RandomVector3(Vector3 bounds)
    {
        return RandomVector3(bounds.x, bounds.y, bounds.z);
    }

    public Vector3 RandomVector3(float x, float y, float z)
    {
        return new Vector3(
            Random.Range(-x, x),
            Random.Range(-y, y),
            Random.Range(-z, z));
    }

    private void ManagePathfinding()
    {
        Pathfinding pf = TryGetPathfinding();
        if (pf == null)
        {
            return;
        }

        //Do pathfinding things
    }
}
