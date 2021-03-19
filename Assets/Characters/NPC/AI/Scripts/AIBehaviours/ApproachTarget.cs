using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI
{
    [AddComponentMenu("Ranchy Rats/AI/ApproachTarget")]
    public class ApproachTarget : AIBehaviour
    {
        public Transform Target;
        public float ChaseRange = 5f;

        public override bool CheckCondition()
        {
            if (Target == null) return false;
            if (Vector3.Distance(Target.position, transform.position) > ChaseRange)
            {
                return false;
            }

            if (HasTimeLimit && timeLeft <= 0f)
            {
                timeLeft = GetTimeLimit();
                return false;
            }
            return true;
        }

        public override void OnUpdate()
        {
            if (HasTimeLimit)
                timeLeft -= Time.deltaTime;
        }
    }
}