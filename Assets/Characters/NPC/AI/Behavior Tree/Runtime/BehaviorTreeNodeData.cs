using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BehaviorTreeNodeData
{
    public string GUID;
    public string Name;
    public List<string> Ports = new List<string>();
    public Vector2 position;
}
