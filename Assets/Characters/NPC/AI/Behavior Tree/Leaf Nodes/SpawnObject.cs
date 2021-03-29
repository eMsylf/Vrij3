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

        public override bool CheckCondition()
        {
            return prefab!=null;
        }

        public override void OnUpdate()
        {
            Instantiate(prefab, position, Quaternion.Euler(rotation), AsChild ? transform : null);
        }

        public override Result Tick()
        {
            if (CheckCondition())
            {
                return Result.Success;
            }
            return Result.Failure;
        }
    }
}