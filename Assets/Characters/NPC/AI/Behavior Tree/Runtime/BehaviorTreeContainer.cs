using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BehaviorTreeContainer : ScriptableObject
{
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    public List<BehaviorTreeNodeData> BTNodeDatas = new List<BehaviorTreeNodeData>();
}
