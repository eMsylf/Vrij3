using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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
        if (!Edges.Any()) return;

        var dialogueContainer = ScriptableObject.CreateInstance<BehaviorTreeContainer>();

        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();

        for (int i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as BehaviorTreeNode;
            var inputNode = connectedPorts[i].input.node as BehaviorTreeNode;

            dialogueContainer.NodeLinks.Add(new NodeLinkData 
            {
                BaseNodeGUID = outputNode.GUID,
                PortName = connectedPorts[i].output.portName,
                TargetNodeGUID = inputNode.GUID
            });
        }

        foreach (var btNode in Nodes.Where(node => !node.EntryPoint))
        {
            dialogueContainer.BTNodeDatas.Add(new BehaviorTreeNodeData
            {
                GUID = btNode.GUID,
                DialogueText = btNode.DialogueText,
                position = btNode.GetPosition().position
            });
        }

        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
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
            var tempNode = _targetGraphView.CreateBehaviorTreeNode(nodeData.DialogueText);
            tempNode.GUID = nodeData.GUID;
            _targetGraphView.AddElement(tempNode);

            var nodePorts = containerCache.NodeLinks.Where(x => x.BaseNodeGUID == nodeData.GUID).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));
        }
    }

    private void ConnectNodes()
    {
        throw new NotImplementedException();
    }
}
