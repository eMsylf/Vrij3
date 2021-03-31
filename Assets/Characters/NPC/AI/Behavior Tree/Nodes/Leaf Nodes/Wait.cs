using RanchyRats.Gyrus.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wait : BTNode
{
    public float Time = 1f;
    private float timeRemaining;

    public Wait(float time, BehaviourController controller = null) : base(controller)
    {
        Time = time;
    }

    public override void Interrupt()
    {
        throw new System.NotImplementedException();
    }

    public override Result Tick()
    {
        if (timeRemaining <= 0f)
        {
            timeRemaining = Time;
            return Result.Success;
        }
        timeRemaining -= UnityEngine.Time.deltaTime;
        return Result.Running;
    }
}
