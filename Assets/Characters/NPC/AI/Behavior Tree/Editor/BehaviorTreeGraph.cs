using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;

public class BehaviorTreeGraph : EditorWindow
{
    private BehaviorTreeGraphView graphView;
    private string fileName = "New Narrative";

    [MenuItem("Tools/Ranchy Rats/Behavior Tree Editor")]
    public static void ShowWindow()
    {
        GetWindow<BehaviorTreeGraph>("Behavior Tree");
    }

    private void OnEnable()
    {
        ConstructGraph();
        GenerateToolbar();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }

    private void ConstructGraph()
    {
        graphView = new BehaviorTreeGraphView
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

        var nodeCreateionButton = new Button(clickEvent: () => { graphView.CreateNode("Behavior Tree Node"); });
        nodeCreateionButton.text = "Create node";
        toolbar.Add(nodeCreateionButton);

        rootVisualElement.Add(toolbar);
    }

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
