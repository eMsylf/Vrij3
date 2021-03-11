using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Cone : MonoBehaviour
{
    [Min(0)]
    public float radius = 10;
    [Range(0, 360)]
    public float angle = 60;
    public Color indicatorColor = Color.white;
    public Color highlightColor = Color.red;

    public List<Vector3> positionsInsideCone = new List<Vector3>();

    public void Check()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = indicatorColor;
        float halfAngle = angle * .5f;
        Handles.DrawSolidArc(transform.position, transform.up, transform.forward, halfAngle, radius);
        Handles.DrawSolidArc(transform.position, transform.up, transform.forward, -halfAngle, radius);

        Handles.color = highlightColor;
        foreach (Vector3 pos in positionsInsideCone)
        {
            Handles.DrawLine(transform.position, pos);
            Handles.DrawSolidDisc(pos, transform.up, 1);
        }
    }
}