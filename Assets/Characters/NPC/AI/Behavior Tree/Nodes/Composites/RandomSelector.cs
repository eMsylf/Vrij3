using RanchyRats.Gyrus.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomSelector : BTComposite
{
    //int[] odds;
    //int oddsWhole
    //{
    //    get
    //    {
    //        int total = odds.Sum();
    //        return total;
    //    }
    //}
    //float[] percentileKeys;
    //bool simple;

    // TODO: Add balancing capabilities. Likeliness of each node being selected
    public RandomSelector(BehaviourController controller,
                          //int[] odds,
                          //bool simple,
                          params BTNode[] nodes) : base(controller, nodes)
    {
        //this.odds = odds;
        //this.simple = simple;
        //percentileKeys = new float[odds.Length];
        //for (int i = 0; i < odds.Length; i++)
        //{
        //    float key01 = Mathf.InverseLerp(0, oddsWhole, odds[i]);
        //    percentileKeys[i] = key01;
        //}
    }

    public override Result Tick()
    {
        int selectedNode = Random.Range(0, nodes.Length);
        return nodes[selectedNode].Tick();
        //if (simple)
        //{
        //    int selectedNode = Random.Range(0, nodes.Length);
        //    return nodes[selectedNode].Tick();
        //}
        //else
        //{
        //    float rolledValue = 0f;
        //    rolledValue = Random.Range(0f, 1f);
        //    return SelectNode(rolledValue).Tick();
        //}
    }

    //public BTNode SelectNode(float rolledValue)
    //{
    //    percentileKeys.AsEnumerable().;
    //    return default;
    //}
}
