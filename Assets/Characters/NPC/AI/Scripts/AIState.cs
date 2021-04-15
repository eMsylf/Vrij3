using UnityEngine;
using FMODUnity;

namespace RanchyRats.Gyrus.AI
{
    [System.Serializable]
    public struct AIState
    {
        // Wat moet een AI allemaal kunnen doen in een state?
        public StudioEventEmitter startSound;

        [Tooltip("The amount of time the AI will spend in the Idle state, randomly picked between these values. X = min, y = max")]
        public Vector2 TimeLimit;
        [Min(0)]
        public float VisionRangeModifier;
        [Range(0f, 360f)]
        public float FieldOfViewModifier;

        public AIState(Vector2 timeLimit, float rangeModifier, float fovModifier)
        {
            startSound = null;
            TimeLimit = timeLimit;
            VisionRangeModifier = rangeModifier;
            FieldOfViewModifier = fovModifier;
        }
    }
}
