using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class PlaySound : Action
    {
        public FMODUnity.StudioEventEmitter sound;

        public override bool CheckCondition()
        {
            return sound != null;
        }

        public override void OnUpdate()
        {
            sound.Play();
        }
    }
}