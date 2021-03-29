using RanchyRats.Gyrus.AI.BehaviorTree;
using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    public class Debug : BTNode
    {
        private string text;

        public Debug(BehaviourController controller, string text) : base(controller)
        {
            this.text = text;
        }

        public override void Interrupt() { }

        public override Result Tick()
        {
            UnityEngine.Debug.Log(text);
            return Result.Success;
        }
    }
}