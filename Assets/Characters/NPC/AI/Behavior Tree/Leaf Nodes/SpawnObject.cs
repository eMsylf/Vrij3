using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class SpawnObject : Action
    {
        public GameObject prefab;
        public bool AsChild = true;
        public Vector3 position;
        public Vector3 rotation;

        public SpawnObject(BehaviourController controller) : base(controller)
        {
        }

        public override void Interrupt()
        {

        }

        public override Result Tick()
        {
            if (prefab == null)
            {
                return Result.Failure;
            }
            Object.Instantiate(prefab, position, Quaternion.Euler(rotation), AsChild ? controller.transform : null);
            return Result.Success;
        }
    }
}