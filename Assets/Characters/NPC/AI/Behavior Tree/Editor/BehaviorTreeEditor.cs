using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

public class BehaviorTreeEditor : GraphViewBlackboardWindow
{
    [MenuItem("Tools/Ranchy Rats/Behavior Tree Editor")]
    public static void ShowWindow()
    {
        GetWindow<BehaviorTreeEditor>("Behavior Tree");
    }

    private void OnGUI()
    {
        
    }
}
