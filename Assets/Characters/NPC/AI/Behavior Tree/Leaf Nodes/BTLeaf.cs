using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BobJeltes.Attributes;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public abstract class BTLeaf : BTNode
    {
        protected PlayerController player;

        public virtual void Init(PlayerController player)
        {
            this.player = player;
        }
    }
}