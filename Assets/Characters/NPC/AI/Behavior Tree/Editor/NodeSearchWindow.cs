using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private BehaviorTreeGraphView graphView;
    private EditorWindow window;
    private Texture2D indentation;

    public void Init(BehaviorTreeGraphView graphView, EditorWindow window)
    {
        this.graphView = graphView;
        this.window = window;

        indentation = new Texture2D(1, 1);
        indentation.SetPixel(0, 0, Color.clear);
        indentation.Apply();
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {


        var tree = new List<SearchTreeEntry> {
            new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
            new SearchTreeGroupEntry(new GUIContent("Behavior Tree"), 1),
            new SearchTreeEntry(new GUIContent("Behavior Tree Node",indentation))
            {
                userData = new BehaviorTreeNode(), level = 2
            }
            //new SearchTreeEntry(new GUIContent("Hello world"))
        };
        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        var worldMousePosition = window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent, context.screenMousePosition - window.position.position);
        var localMousePosition = graphView.contentViewContainer.WorldToLocal(worldMousePosition);

        switch (SearchTreeEntry.userData)
        {
            case BehaviorTreeNode behaviorTreeNode:
                Debug.Log("Behavior Tree Node created");
                graphView.CreateNode("Behavior Tree Node", localMousePosition);
                return true;
            default:
                return false;
        }
    }
}
