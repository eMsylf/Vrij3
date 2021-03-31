using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    [System.Serializable]
    public class BTDebug : BTNode
    {
        private string text;

        public BTDebug(string text, BehaviourController controller = null) : base(controller)
        {
            this.text = text;
        }

        public override void Interrupt() { }

        public override Result Tick()
        {
            Debug.Log(text);
            return Result.Success;
        }
    }
}