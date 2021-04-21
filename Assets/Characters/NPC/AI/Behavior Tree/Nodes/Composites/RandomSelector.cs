using RanchyRats.Gyrus.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSelector : BTComposite
{
    int[] odds;
    int oddsTotal
    {
        get
        {
            int total = 0;
            for (int i = 0; i < odds.Length; i++)
            {
                total += odds[i];
            }
            return total;
        }
    }
    int[] nodeLookup;

    // TODO: Add balancing capabilities. Likeliness of each node being selected
    public RandomSelector(BehaviourController controller, int[] odds, params BTNode[] nodes) : base(controller, nodes)
    {
        this.odds = odds;
        nodeLookup = new int[oddsTotal];
        // Voor elke waarde in het totaal...
        for (int i = 0; i < oddsTotal; i++)
        {
            for (int j = 0; j < odds[i]; j++)
            {

                i++;
            }
        }
        nodeLookup = new int[]
        {
            1,
            2
        };
    }

    public override Result Tick()
    {
        //oddsTotal
        int selectedNode = Random.Range(0, nodes.Length - 1);
        return nodes[selectedNode].Tick();
    }
}
