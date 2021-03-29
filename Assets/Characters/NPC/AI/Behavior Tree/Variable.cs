using UnityEngine;

namespace RanchyRats.Gyrus.AI.BehaviorTree
{
    [CreateAssetMenu()]
    public class Variable : ScriptableObject
    {
        public string Name;
        public string Type;
        [BobJeltes.Attributes.Button("Set type")]
        public object value;
        public virtual T GetValue<T>()
        {
            return (T)value;
        }
    }
}