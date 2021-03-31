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
                return Result.Failure;
            
            if (waitToFinish)
            {
                // If the sound is not playing but the "started" flag was set, return a success to indicate the sound has finished playing.
                if (!sound.IsPlaying())
                {
                    if (started)
                    {
                        started = false;
                        return Result.Success;
                    }
                    // Otherwise, play the sound and set the "started" flag.
                    sound.Play();
                    started = true;
                }
                // If the sound is still playing, the node is still running.
                return Result.Running;
            }

            sound.Play();
            return Result.Success;
        }
    }
}