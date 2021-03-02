using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Direction : MonoBehaviour
{
    public Transform DirectionIndicator;
    public float distance = 1f;
    public void UpdatePosition(Vector3 position)
    {
        DirectionIndicator.localPosition = position * distance;
    }

    //public LookDirection DefaultDirection;
    //public List<LookDirection> Directions;

    //public ELookDirection LookDirection;

    //public ELookDirection UpdateLookDirection(Vector2 movement)
    //{
    //    // Set movement indicator local position
    //    DirectionIndicator.localPosition = new Vector3(movement.x, 0f, movement.y);

    //    // If no input is given, return the default lookdirection (South by default)
    //    if (movement == Vector2.zero)
    //        return default;

    //    // Define a distance to beat, that can be commpared to by new measurements to see which look direction is closest to the input movement
    //    float distanceToBeat = Mathf.Infinity;
    //    LookDirection closestLookDirection = null;
    //    // Look through all lookdirection objects
    //    foreach (LookDirection lookDirectionObj in Directions)
    //    {
    //        float distanceToDirection = Vector3.Distance(DirectionIndicator.position, lookDirectionObj.transform.position);
    //        if (distanceToDirection < distanceToBeat)
    //        {
    //            distanceToBeat = distanceToDirection;
    //            closestLookDirection = lookDirectionObj;
    //        }
    //    }
    //    //Debug.Log("Update move direction: " + closestLookDirection.Direction);
    //    LookDirection = closestLookDirection.Direction;

    //    return closestLookDirection.Direction;
    //}

    //public ELookDirection GetLookDirection()
    //{
    //    return LookDirection;
    //}

    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawLine();
    }
}
//public enum ELookDirection
//{
//    N,
//    NE,
//    E,
//    SE,
//    S,
//    SW,
//    W,
//    NW
//}
