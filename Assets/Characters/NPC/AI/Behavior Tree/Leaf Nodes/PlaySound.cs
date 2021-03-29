using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class PlaySound : Action
    {
        public FMODUnity.StudioEventEmitter sound;
        public bool WaitToFinish;

        public PlaySound(BehaviourController controller, FMODUnity.StudioEventEmitter sound) : base(controller)
        {
            this.sound = sound;
        }

        public override void Interrupt()
        {
            sound?.Stop();
        }

        public override Result Tick()
        {
            if (sound == null)
            {
                return Result.Failure;
            }
            sound.Play();
            return Result.Success;
        }
    }
}