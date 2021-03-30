using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class PlaySound : BTNode
    {
        private FMODUnity.StudioEventEmitter sound;
        private bool waitToFinish;
        private bool started;

        public PlaySound(BehaviourController controller, FMODUnity.StudioEventEmitter sound, bool waitToFinish) : base(controller)
        {
            this.sound = sound;
            this.waitToFinish = waitToFinish;
        }

        public override void Interrupt()
        {
            sound?.Stop();
            started = false;
        }

        public override Result Tick()
        {
            if (sound == null)
            {
                return Result.Failure;
            }

            if (waitToFinish)
            {
                if (!sound.IsPlaying())
                {
                    if (started)
                    {
                        started = false;
                        return Result.Success;
                    }
                    sound.Play();
                    started = true;
                }
                return Result.Running;
            }

            sound.Play();
            return Result.Success;
        }
    }
}