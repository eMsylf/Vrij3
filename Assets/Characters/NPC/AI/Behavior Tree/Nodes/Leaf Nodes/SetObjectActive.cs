using RanchyRats.Gyrus.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetObjectActive : BTNode
{
    private GameObject gameObject;
    private bool activate;
    public SetObjectActive(GameObject gameObject, bool activate, BehaviourController controller = null) : base(controller)
    {
        this.gameObject = gameObject;
        this.activate = activate;
    }

    public override void Interrupt() { }

    public override Result Tick()
    {
        if (gameObject == null) return Result.Failure;
        gameObject.SetActive(activate);
        return Result.Success;
    }
}
