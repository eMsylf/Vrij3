using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BobJeltes.Attributes;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public abstract class Action : BTNode
    {
        protected Action(BehaviourController controller) : base(controller)
        {
        }
    }
}