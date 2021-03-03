﻿namespace RanchyRats.Gyrus
{
    public partial class Movement
    {
        // TODO: Omzetten in Scriptable Object?
        [System.Serializable]
        public struct MovementStateSettings
        {
            public float speed;
            // State duration
            public bool expires;
            public float duration;
            public State nextState;
            // Stamina drain
            public bool drainsStamina;
            public float staminaDrainAmount;
            public float staminaDrainInterval;

            public MovementStateSettings(float speed, bool expires = false, float duration = 0f, State nextState = State.Idle, bool drainsStamina = false, float staminaDrainAmount = 0f, float staminaDrainInterval = 1f)
            {
                this.speed = speed;
                this.expires = expires;
                this.duration = duration;
                this.nextState = nextState;
                this.drainsStamina = drainsStamina;
                this.staminaDrainAmount = staminaDrainAmount;
                this.staminaDrainInterval = staminaDrainInterval;
            }
        }
    }
}