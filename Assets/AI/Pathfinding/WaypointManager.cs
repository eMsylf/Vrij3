using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public Transform GetRandomWaypoint()
    {
        return transform.GetChild(Random.Range(0, transform.childCount - 1));
    }

    public Transform GetNextWaypoint(int current)
    {
        current++;
        if (current >= transform.childCount)
        {
            current = 0;
        }
        return transform.GetChild(current);
    }

    public Transform GetPreviousWaypoint(int current)
    {
        current--;
        if (current < 0)
        {
            current = transform.childCount - 1;
        }
        return transform.GetChild(current);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, GetNextWaypoint(i).position);
        }
    }
#endif
}