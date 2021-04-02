// Source: Mert Kirimgeri (YouTube)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviorTreeGraphView : GraphView
{
    public readonly Vector2 defaultNodeSize = new Vector2(150, 200);
    public Blackboard Blackboard;
    public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
    private NodeSearchWindow _searchWindow;

    public BehaviorTreeGraphView(EditorWindow editorWindow)
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
        AddSearchWindow(editorWindow);
    }

    public void ClearBlackboardWithProperties()
    {
        ExposedProperties.Clear();
        Blackboard.Clear();
    }

    internal void AddPropertyToBlackboard(ExposedProperty exposedProperty)
    {
        var localPropertyName = exposedProperty.PropertyName;
        var localPropertyValue = exposedProperty.PropertyValue;

        while (ExposedProperties.Any(x => x.PropertyName == localPropertyName))
        {
            localPropertyName = $"{localPropertyName}(1)";
        }

        var property = new ExposedProperty();
        property.PropertyName = localPropertyName;
        property.PropertyValue = localPropertyValue;

        ExposedProperties.Add(property);

        var container = new VisualElement();
        var blackboardField = new BlackboardField { text = property.PropertyName, typeText = "string" };
        container.Add(blackboardField);

        var propertyValueTextField = new TextField("Value:")
        {
            value = localPropertyValue
        };
        propertyValueTextField.RegisterValueChangedCallback(evt =>
        {
            var changingPropertyIndex = ExposedProperties.FindIndex(x => x.PropertyName == property.PropertyName);
            ExposedProperties[changingPropertyIndex].PropertyValue = evt.newValue;
        });
        var blackboardValueRow = new BlackboardRow(blackboardField, propertyValueTextField);
        container.Add(blackboardValueRow);

        Blackboard.Add(container);
    }

    private void AddSearchWindow(EditorWindow editorWindow)
    {
        _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();

        _searchWindow.Init(this, editorWindow);

        nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
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
            IsRoot = true
        };

        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);


        var tickBox = new Toggle("Lock");
        tickBox.RegisterValueChangedCallback((val) => LockRootNode(node, val.newValue));
        //tickBox.RegisterValueChangedCallback(() => locked);
        node.titleContainer.Add(tickBox);
        if (locked)
            node.capabilities &= ~Capabilities.Movable;
        node.capabilities &= ~Capabilities.Deletable;

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(x: 0, y: 0, width: 100, height: 150));
        return node;
    }

    bool locked = true;
    public void LockRootNode(BehaviorTreeNode node, bool locked)
    {
        this.locked = locked;
        if (!this.locked)
            node.capabilities |= Capabilities.Movable;
        else
            node.capabilities &= ~Capabilities.Movable;
    }

    public void CreateNode(string nodeName, Vector2 position)
    {
        AddElement(CreateBehaviorTreeNode(nodeName, position));
    }

    internal BehaviorTreeNode CreateBehaviorTreeNode(string nodeName, Vector2 position)
    {
        var behaviorTreeNode = new BehaviorTreeNode
        {
            title = nodeName,
            DialogueText = nodeName,
            GUID = Guid.NewGuid().ToString(),
        };

        var inputPort = GeneratePort(behaviorTreeNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        behaviorTreeNode.inputContainer.Add(inputPort);

        behaviorTreeNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

        var button = new Button(() => { AddChoicePort(behaviorTreeNode); });
        button.text = "New Choice"; 
        behaviorTreeNode.titleContainer.Add(button);

        var textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt =>
        {
            behaviorTreeNode.DialogueText = evt.newValue;
            behaviorTreeNode.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(behaviorTreeNode.title);
        behaviorTreeNode.mainContainer.Add(textField);

        behaviorTreeNode.RefreshExpandedState();
        behaviorTreeNode.RefreshPorts();
        behaviorTreeNode.SetPosition(new Rect(position, defaultNodeSize));
        return behaviorTreeNode;
    }

    public void AddChoicePort(BehaviorTreeNode behaviorTreeNode, string overriddenPortName = "")
    {
        var generatedPort = GeneratePort(behaviorTreeNode, Direction.Output);

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);

        var outputPortCount = behaviorTreeNode.outputContainer.Query("connector").ToList().Count;

        var choicePortName = string.IsNullOrEmpty(overriddenPortName) ? $"Choice {outputPortCount}":overriddenPortName;

        behaviorTreeNode.Ports.Add(choicePortName);

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
        behaviorTreeNode.Ports.Remove(generatedPort.portName);
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
