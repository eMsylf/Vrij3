using RanchyRats.Gyrus.AI.BehaviorTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Behavior Tree", menuName = "Ranchy Rats/AI/Behavior Tree")]
public class TreeCreator : ScriptableObject
{
    public List<BTDebug> nodes = new List<BTDebug>();
}
