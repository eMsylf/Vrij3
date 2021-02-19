using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BobJeltes.Extensions;

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

    public override void Update()
    {
        base.Update();
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

        rb.AddForce(Extensions.RandomVector3(JitterAmount));
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
