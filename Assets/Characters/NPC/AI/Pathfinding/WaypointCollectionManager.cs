using BobJeltes.StandardUtilities;
using UnityEngine;

public class WaypointCollectionManager : Singleton<WaypointCollectionManager>
{
    WaypointCollection[] WaypointCollections;


    public bool WarnOnlyOnce = true;
    private bool warningGiven = false;
    public WaypointCollection[] GetWaypointCollections(bool redefineList)
    {
        if (redefineList || WaypointCollections == null)
        {
            WaypointCollections = FindObjectsOfType<WaypointCollection>();
        }

        if (WaypointCollections.Length == 0)
        {
            if (!warningGiven)
            {
                Debug.LogWarning("No waypoint collections found in scene!", this);
                if (WarnOnlyOnce) warningGiven = true;
            }
            else if (!WarnOnlyOnce) warningGiven = false;
        }
        else
        {
            for (int i = 0; i < WaypointCollections.Length; i++)
            {
                if (WaypointCollections[i] == null)
                {
                    Debug.LogWarning("Waypoint collection at index " + i + " is null. Redefining list...", this);
                    WaypointCollections = FindObjectsOfType<WaypointCollection>();
                    break;
                }
            }
            
        }
        return WaypointCollections;
    }

    public WaypointCollection GetClosestWaypointCollection(Vector3 position)
    {
        WaypointCollection[] wpCollections = GetWaypointCollections(false);
        if (wpCollections == null || wpCollections.Length == 0)
        {
            return null;
        }

        float closestDistance = Mathf.Infinity;
        WaypointCollection closestWaypointCollection = wpCollections[0];
        for (int i = 0; i < wpCollections.Length; i++)
        {
            float distance = Vector3.Distance(position, wpCollections[i].transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestWaypointCollection = wpCollections[i];
            }
        }
        return closestWaypointCollection;
    }
}
