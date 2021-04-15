using BobJeltes;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Gyrus.Combat
{
    [Serializable]
    public class Stat : MonoBehaviour
    {
        [SerializeField]
        private float m_maxValue = 4;
        [SerializeField]
        private float m_value;
        public bool AllowOverflow;
        public bool AllowUnderflow;
        [Tooltip("When enabled, this statistic is set to its maximum automatically when Start() is called.")]
        public bool setValueToMax = true;
        [Header("Recovery")]
        [Tooltip("The time it takes for a point to be recovered..")]
        public float recoveryTime = 1f;
        public float recoveryWindup = 1f;
        private float windup;
        private float recovery;
        public bool allowRecovery = false;

        public float MaxValue
        {
            get => m_maxValue;
            set => m_maxValue = value;
        }

        public float Value
        {
            get => m_value;
            set => m_value = value;
        }

        public FloatEvent OnValueChanged;
        public UnityEvent OnDepleted;
        public UnityEvent OnIncrease;
        public UnityEvent OnDecrease;

        public struct Sounds
        {
            public FMODUnity.StudioEventEmitter depleted;
            public FMODUnity.StudioEventEmitter recovery;
            public FMODUnity.StudioEventEmitter recoverToFull;
        }
        public Sounds sounds;

        public bool IsEmpty(bool playWarningSound)
        {
            if (m_value <= 0)
            {
                if (playWarningSound)
                {
                    sounds.depleted.Play();
                }
            }
            return m_value <= 0;
        }

        public void SetCurrent(float value)
        {
            if (Value == value)
                return;

            if (value > Value)
            {
                if (!AllowOverflow && value > MaxValue)
                    value = MaxValue;

                if (value >= MaxValue)
                    OnIncrease.Invoke();
            }
            else
            {
                if (!AllowUnderflow && value < 0)
                    value = 0;
                OnDecrease.Invoke();
            }

            Value = value;
            //Debug.Log("Set " + name + " to " + _value + " points", this);
            OnValueChanged.Invoke(value);
        }

        public void Use(float amount)
        {
            if (Value - amount >= 0)
            {
                SetCurrent(m_value - amount);
            }

            if (m_value <= 0 && OnDepleted != null)
                OnDepleted.Invoke();
        }

        private void Start()
        {
            if (setValueToMax)
            {
                Value = MaxValue;
            }
        }

        private void Update()
        {
            ManageRecharge();
        }

        void ManageRecharge()
        {
            if (!allowRecovery)
                return;
            if (m_value < m_maxValue)
            {
                if (windup < recoveryWindup)
                {
                    windup += Time.deltaTime;
                }
                else
                {
                    if (recovery < recoveryTime)
                    {
                        recovery += Time.deltaTime;
                    }
                    else
                    {
                        SetCurrent(m_value + 1);
                        recovery = 0f;
                    }
                }
            }
        }
    }
}
