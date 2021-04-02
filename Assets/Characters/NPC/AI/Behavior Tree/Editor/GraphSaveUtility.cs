// Source: Mert Kirimgeri (YouTube)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphSaveUtility
{
    private BehaviorTreeGraphView _targetGraphView;
    private BehaviorTreeContainer containerCache;

    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<BehaviorTreeNode> Nodes => _targetGraphView.nodes.ToList().Cast<BehaviorTreeNode>().ToList();
    public static GraphSaveUtility GetInstance(BehaviorTreeGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(string fileName)
    {
        var behaviorTreeContainer = ScriptableObject.CreateInstance<BehaviorTreeContainer>();
        if (!SaveNodes(behaviorTreeContainer)) return;
        SaveExposedProperties(behaviorTreeContainer);
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        AssetDatabase.CreateAsset(behaviorTreeContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    private void SaveExposedProperties(BehaviorTreeContainer behaviorTreeContainer)
    {
        behaviorTreeContainer.ExposedProperties.AddRange(_targetGraphView.ExposedProperties);
    }

    private bool SaveNodes(BehaviorTreeContainer behaviorTreeContainer)
    {
        if (!Edges.Any()) return false;


        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();

        for (int i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as BehaviorTreeNode;
            var inputNode = connectedPorts[i].input.node as BehaviorTreeNode;

            behaviorTreeContainer.NodeLinks.Add(new NodeLinkData
            {
                BaseNodeGUID = outputNode.GUID,
                PortName = connectedPorts[i].output.portName,
                TargetNodeGUID = inputNode.GUID
            });
        }

        foreach (var btNode in Nodes.Where(node => !node.EntryPoint))
        {
            behaviorTreeContainer.BTNodeDatas.Add(new BehaviorTreeNodeData
            {
                GUID = btNode.GUID,
                DialogueText = btNode.DialogueText,
                position = btNode.GetPosition().position
            });
        }
        return true;
    }

    public void LoadGraph(string fileName)
    {
        containerCache = Resources.Load<BehaviorTreeContainer>(fileName);

        if (containerCache== null)
        {
            EditorUtility.DisplayDialog("File not found", "Target dialogue graph file does not exist!", "OK");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
        CreateExposedProperties();
    }

    private void CreateExposedProperties()
    {
        // Clear existing properties on hot-reload
        _targetGraphView.ClearBlackboardWithProperties();
        // Add properties from data
        foreach (var exposedProperty in containerCache.ExposedProperties)
        {
            _targetGraphView.AddPropertyToBlackboard(exposedProperty);
        }
    }

    private void ClearGraph()
    {
        // Set entry points guid back from the save. Discard existing guid.
        Nodes.Find(x => x.EntryPoint).GUID = containerCache.NodeLinks[0].BaseNodeGUID;

        foreach (var node in Nodes)
        {
            if (node.EntryPoint) continue;
            Edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));

            _targetGraphView.RemoveElement(node);
        }
    }

    private void CreateNodes()
    {
        foreach (var nodeData in containerCache.BTNodeDatas)
        {
            var tempNode = _targetGraphView.CreateBehaviorTreeNode(nodeData.DialogueText, Vector2.zero);
            tempNode.GUID = nodeData.GUID;
            _targetGraphView.AddElement(tempNode);

            var nodePorts = containerCache.NodeLinks.Where(x => x.BaseNodeGUID == nodeData.GUID).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));
        }
    }

    private void ConnectNodes()
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            var connections = containerCache.NodeLinks.Where(x => x.BaseNodeGUID == Nodes[i].GUID).ToList();
            for (int j = 0; j < connections.Count; j++)
            {
                var targetNodeGUID = connections[j].TargetNodeGUID;
                var targetNode = Nodes.First(XboxBuildSubtarget => XboxBuildSubtarget.GUID == targetNodeGUID);
                // Error: When no nodes are connected to the root node
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);

                targetNode.SetPosition(new Rect(
                        containerCache.BTNodeDatas.First(x => x.GUID == targetNodeGUID).position,
                        _targetGraphView.defaultNodeSize
                    ));
            }
        }
    }

    private void LinkNodes(Port port1, Port port2)
    {
        var tempEdge = new Edge
        {
            output = port1,
            input = port2
        };

        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);

        _targetGraphView.Add(tempEdge);
    }
}
