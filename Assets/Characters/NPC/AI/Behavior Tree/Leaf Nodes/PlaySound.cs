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

        public override Result Tick()
        {
            if (CheckCondition())
            {
                return Result.Failure;
            }
            else if (sound.IsPlaying()) // Hier? In update? Blijven checken of het speelt, of gewoon 1 keer spelen en dan success returnen?
            {
                return Result.Running;
            }
            return Result.Success;


            //if (CheckCondition())
            //{
            //    return Result.Failure;
            //}
            //OnUpdate();
            //return Result.Success;
        }
    }
}