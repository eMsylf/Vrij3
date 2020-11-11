using BobJeltes.StandardUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManagerCollection : Singleton<WaypointManagerCollection>
{
    WaypointManager[] WaypointManagers;

    protected bool WaypointManagersCollected = false;
    public WaypointManager[] GetWaypointManagers(bool redefineList)
    {
        if (redefineList || WaypointManagers == null)
        {
            WaypointManagers = FindObjectsOfType<WaypointManager>();
            if (WaypointManagers.Length == 0)
            {
                Debug.LogError("No waypointmanagers found in scene!", this);
            }
        }
        return WaypointManagers;
    }

    public WaypointManager GetClosestWaypointManager(Vector3 position)
    {
        WaypointManager[] wpManagers = GetWaypointManagers(false);
        if (wpManagers == null || wpManagers.Length == 0)
        {
            return null;
        }

        float closestDistance = Mathf.Infinity;
        WaypointManager closestWaypointManager = wpManagers[0];
        for (int i = 0; i < wpManagers.Length; i++)
        {
            float distance = Vector3.Distance(position, wpManagers[i].transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestWaypointManager = wpManagers[i];
            }
        }
        return closestWaypointManager;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

}
