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
    public Color handleColor = Color.white;
    public bool handleFill = true;
    //public Color highlightColor = Color.red;
    public LayerMask layermMask = new LayerMask();

    //public List<Vector3> positionsInsideCone = new List<Vector3>();

    public void Check()
    {
        
    }

    public void Draw(Vector3 origin, Vector3 direction, Vector3 orientation)
    {
        Handles.color = handleColor;
        float halfAngle = angle * .5f;
        if (handleFill)
        {
            Handles.DrawSolidArc(origin, orientation, direction, halfAngle, radius);
            Handles.DrawSolidArc(origin, orientation, direction, -halfAngle, radius);
        }

        //Handles.color = highlightColor;
        //foreach (Vector3 pos in positionsInsideCone)
        //{
        //    Handles.DrawLine(origin, pos);
        //    Handles.DrawSolidDisc(pos, orientation, 1);
        //}
    }
}