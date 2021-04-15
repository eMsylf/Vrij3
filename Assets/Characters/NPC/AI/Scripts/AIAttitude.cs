using UnityEngine;

namespace RanchyRats.Gyrus
{
    [CreateAssetMenu(menuName = "Ranchy Rats/AI/Attitude")]
    public class AIAttitude : ScriptableObject
    {
        [SerializeField]
#pragma warning disable CS0649
        private LayerMask aggressionLayers;
#pragma warning restore
        public LayerMask AggressionLayers { get => aggressionLayers; }
    }
}