using RanchyRats.Gyrus.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wait : Action
{
    public float Time = 1f;
    private float timeRemaining;

    public override bool CheckCondition()
    {
        timeRemaining = Time;
        return true;
    }

    public override void OnUpdate()
    {
        timeRemaining -= UnityEngine.Time.deltaTime;
    }

    public override Result Tick()
    {
        if (timeRemaining <= 0f)
        {
            return Result.Success;
        }
        return Result.Running;
    }
}
