// Source: Mert Kirimgeri (YouTube)
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BehaviorTreeNode : Node
{
    public string GUID;

    public string DialogueText;

    public List<string> Ports = new List<string>();

    public bool IsRoot = false;
}
