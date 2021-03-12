using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Cone
{
    [Min(0)]
    public float radius = 10;
    [Range(0, 360)]
    public float angle = 60;
    public Color indicatorColor = Color.white;
    public Color highlightColor = Color.red;
    public LayerMask layermMask = new LayerMask();

    public List<Vector3> positionsInsideCone = new List<Vector3>();

    private Vector3 origin = Vector3.zero;
    private Vector3 direction = Vector3.forward;
    private Vector3 orientation = Vector3.up;

    public void Check()
    {
        
    }

    public void SetOrigin(Vector3 origin)
    {
        this.origin = origin;
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }

    public void SetOrientation(Vector3 orientation)
    {
        this.orientation = orientation;
    }

    public void Draw(Vector3 origin, Vector3 direction, Vector3 orientation)
    {
        Handles.color = indicatorColor;
        float halfAngle = angle * .5f;
        Handles.DrawSolidArc(origin, orientation, direction, halfAngle, radius);
        Handles.DrawSolidArc(origin, orientation, direction, -halfAngle, radius);

        Handles.color = highlightColor;
        foreach (Vector3 pos in positionsInsideCone)
        {
            Handles.DrawLine(origin, pos);
            Handles.DrawSolidDisc(pos, orientation, 1);
        }
    }
}