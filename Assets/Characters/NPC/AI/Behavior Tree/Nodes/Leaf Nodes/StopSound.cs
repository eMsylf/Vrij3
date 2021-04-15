using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class StopSound : BTNode
    {
        private FMODUnity.StudioEventEmitter sound;

        public StopSound(BehaviourController controller, FMODUnity.StudioEventEmitter sound) : base(controller)
        {
            this.sound = sound;
        }

        public override void Interrupt() { }

        public override Result Tick()
        {
            if (sound == null)
            {
                return Result.Failure;
            }

            sound.Stop();
            return Result.Success;
        }
    }
}