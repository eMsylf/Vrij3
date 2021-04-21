using RanchyRats.Gyrus;
using RanchyRats.Gyrus.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    public override void Die()
    {
        base.Die();
        StartDeathAnimation();
    }

    public void StartDeathAnimation()
    {
        if (Animator == null)
        {
            Debug.LogError("Animator not assigned", gameObject);
            return;
        }

        Animator.SetTrigger("Death");
    }
}
