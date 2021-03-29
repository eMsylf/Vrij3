using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public abstract class BTComposite : BTNode
    {
        public BTNode[] nodes; // TODO: The selector can now only carry references to nodes, which excludes being able to paste actions directly into it. Find a way to be able to reference actions 
        protected int i = 0;
    }
}