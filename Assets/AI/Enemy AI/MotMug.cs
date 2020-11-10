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

    private void Update()
    {
        ManagePathfinding();
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
