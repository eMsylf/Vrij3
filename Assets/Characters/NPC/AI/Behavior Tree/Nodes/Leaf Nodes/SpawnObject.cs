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

        public SpawnObject(BehaviourController controller, GameObject prefab, bool asChild = false, Vector3 position = default, Vector3 rotation = default) : base(controller)
        {
            this.prefab = prefab;
            AsChild = asChild;
            this.position = position;
            this.rotation = rotation;
        }

        public override void Interrupt() { }

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