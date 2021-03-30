using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviorTreeGraphView : GraphView
{
    private readonly Vector2 defaultNodeSize = new Vector2(150, 200);

    public BehaviorTreeGraphView()
    {
        styleSheets.Add(Resources.Load<StyleSheet>("BehaviorTreeGraph"));

        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        AddElement(GenerateRootNode());
    }

    private Port GeneratePort(BehaviorTreeNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, type: typeof(float)); // Arbitrary type
    }

    public BehaviorTreeNode GenerateRootNode()
    {
        BehaviorTreeNode node = new BehaviorTreeNode
        {
            title = "Root",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = "Root Node",
            EntryPoint = true
        };

        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(x: 100, y: 200, width: 100, height: 150));
        return node;
    }

    public void CreateNode(string nodeName)
    {
        AddElement(CreateBehaviorTreeNode(nodeName));
    }

    internal BehaviorTreeNode CreateBehaviorTreeNode(string nodeName)
    {
        var behaviorTreeNode = new BehaviorTreeNode
        {
            title = nodeName,
            DialogueText = nodeName,
            GUID = Guid.NewGuid().ToString()
        };

        var inputPort = GeneratePort(behaviorTreeNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        behaviorTreeNode.inputContainer.Add(inputPort);

        var button = new Button(() => { AddChoicePort(behaviorTreeNode); });
        button.text = "New Choice"; 
        behaviorTreeNode.titleContainer.Add(button);

        behaviorTreeNode.RefreshExpandedState();
        behaviorTreeNode.RefreshPorts();
        behaviorTreeNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));
        return behaviorTreeNode;
    }

    public void AddChoicePort(BehaviorTreeNode behaviorTreeNode, string overriddenPortName = "")
    {
        var generatedPort = GeneratePort(behaviorTreeNode, Direction.Output);

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);

        var outputPortCount = behaviorTreeNode.outputContainer.Query("connector").ToList().Count;

        var choicePortName = string.IsNullOrEmpty(overriddenPortName) ? $"Choice {outputPortCount}":overriddenPortName;

        var textField = new TextField{
            name = string.Empty,
            value = choicePortName
        };
        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label(" "));
        generatedPort.contentContainer.Add(textField);
        var deleteButton = new Button(() => RemovePort(behaviorTreeNode, generatedPort))
        {
            text = "X"
        };
        generatedPort.contentContainer.Add(deleteButton);

        generatedPort.portName = choicePortName;

        behaviorTreeNode.outputContainer.Add(generatedPort);
        behaviorTreeNode.RefreshPorts();
        behaviorTreeNode.RefreshExpandedState();
    }

    private void RemovePort(BehaviorTreeNode behaviorTreeNode, Port generatedPort)
    {
        var targetEdge = edges.ToList().Where(x => x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }

        behaviorTreeNode.outputContainer.Remove(generatedPort);
        behaviorTreeNode.RefreshPorts();
        behaviorTreeNode.RefreshExpandedState();
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach((port) =>
        {
            if (startPort != port && startPort.node != port.node)
            {
                compatiblePorts.Add(port);
            }
        });

        return compatiblePorts;
    }
}
