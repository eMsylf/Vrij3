// Source: Mert Kirimgeri (YouTube)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using System.Linq;

public class BehaviorTreeGraph : EditorWindow
{
    private BehaviorTreeGraphView graphView;
    private string fileName = "New Narrative";
    private static BehaviorTreeGraph instance;

    [MenuItem("Tools/Ranchy Rats/Behavior Tree Editor")]
    public static void ShowWindow()
    {
        instance = GetWindow<BehaviorTreeGraph>("Behavior Tree");
    }

    private void OnEnable()
    {
        ConstructGraph();
        GenerateToolbar();
        GenerateMinimap();
        GenerateBlackboard();
    }

    private void GenerateBlackboard()
    {
        var blackboard = new Blackboard(graphView);
        blackboard.Add(new BlackboardSection { title = "Exposed properties" });
        blackboard.addItemRequested = _blackboard => {graphView.AddPropertyToBlackboard(new ExposedProperty());};
        blackboard.editTextRequested = (blackboard1, element, newValue) =>
        {
            var oldPropertyName = ((BlackboardField)element).text;
            if (graphView.ExposedProperties.Any(x => x.PropertyName == newValue))
            {
                EditorUtility.DisplayDialog("Error", "This property name already exists, please choose another one.", "OK");
                return;
            }

            var propertyIndex = graphView.ExposedProperties.FindIndex(x => x.PropertyName == oldPropertyName);
            graphView.ExposedProperties[propertyIndex].PropertyName = newValue;
            ((BlackboardField)element).text = newValue;
        };

        blackboard.SetPosition(new Rect(10, 30, 200, 300));

        graphView.Add(blackboard);
        graphView.Blackboard = blackboard;
    }

    private void GenerateMinimap()
    {
        var miniMap = new MiniMap { anchored = true };
        var cords = graphView.contentViewContainer.WorldToLocal(new Vector2(maxSize.x -1, 30));
        miniMap.SetPosition(new Rect(cords.x, cords.y, 200, 140));
        graphView.Add(miniMap);
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }

    private void ConstructGraph()
    {
        graphView = new BehaviorTreeGraphView(this)
        {
            name = "Behavior Tree Graph"
        };
        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    public void GenerateToolbar()
    {
        Toolbar toolbar = new Toolbar();

        var fileNameTextField = new TextField("File name: ");
        fileNameTextField.SetValueWithoutNotify(fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => fileName = evt.newValue);
        toolbar.Add(fileNameTextField);

        toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save Data" });
        toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load Data" });

        //var nodeCreateionButton = new Button(clickEvent: () => { graphView.CreateNode("Behavior Tree Node"); });
        //nodeCreateionButton.text = "Create node";
        //toolbar.Add(nodeCreateionButton);

        rootVisualElement.Add(toolbar);
    }

    //[UnityEditor.Callbacks.DidReloadScripts]
    //private static void OnScriptsReloaded()
    //{
    //    Debug.Log("Scripts reloaded.");
    //    instance?.RequestDataOperation(false);
    //}

    private void RequestDataOperation(bool save)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name.", "OK!");
            return;
        }

        var saveUtility = GraphSaveUtility.GetInstance(graphView);
        if (save)
        {
            saveUtility.SaveGraph(fileName);
        }
        else
        {
            saveUtility.LoadGraph(fileName);
        }
    }
}
