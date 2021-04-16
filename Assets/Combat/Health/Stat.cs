using BobJeltes;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Gyrus.Combat
{
    [Serializable]
    public class Stat : MonoBehaviour
    {
        [SerializeField] [Min(0)]
        private float m_maxValue = 4;
        [SerializeField]
        private float m_value;
        public bool AllowOverflow;
        public bool AllowUnderflow;
        [Tooltip("When enabled, this statistic is set to its maximum automatically when Start() is called.")]
        public bool setValueToMax = true;
        [Header("Recharge")]
        public bool allowRecharge = false;
        [Min(0)][Tooltip("The time it takes for the recharge time to start counting, after using this value")]
        public float rechargeWindupTime = 1f;
        [Min(0)] [Tooltip("The time it takes for a point to be recharged")]
        public float rechargeTime = 1f;
        [Min(0)]
        public float rechargeAmount = 1f;
        private float windupRemaining;
        private float timeBeforeRecharge;

        public float MaxValue
        {
            get => m_maxValue;
            set => m_maxValue = value;
        }
        public float Value
        {
            get => m_value;
            set
            {
                if (m_value == value)
                    return;

                if (value > m_value)
                {
                    if (!AllowOverflow && value > MaxValue)
                        value = MaxValue;
                    OnValueIncrease.Invoke();
                }
                else
                {
                    if (!AllowUnderflow && value < 0)
                        value = 0;
                    OnValueDecrease.Invoke();
                }

                m_value = value;
                OnValueChanged.Invoke(value);
            }
        }

        public FloatEvent OnValueChanged;
        public UnityEvent OnValueIncrease;
        public UnityEvent OnValueDecrease;
        public UnityEvent OnDepleted;

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
            if (!allowRecharge)
                return;
            if (m_value >= m_maxValue)
            {
                return;
            }
            if (windupRemaining > 0f)
            {
                windupRemaining -= Time.deltaTime;
                return;
            }
            if (timeBeforeRecharge > 0f)
            {
                timeBeforeRecharge -= Time.deltaTime;
                return;
            }
            Value = m_value + rechargeAmount;
            timeBeforeRecharge = rechargeTime;
        }

        public bool IsEmpty(bool fireEvent = true)
        {
            if (fireEvent) OnDepleted.Invoke();
            return m_value <= 0;
        }

        public void SetValueWithoutEvent(float value)
        {
            if (Value == value)
                return;

            if (value > Value && !AllowOverflow && value > MaxValue)
                value = MaxValue;
            else if (!AllowUnderflow && value < 0)
                    value = 0;

            Value = value;
            //Debug.Log("Set " + name + " to " + _value + " points", this);
            OnValueChanged.Invoke(value);
        }

        public void Use(float amount)
        {
            if (Value - amount >= 0)
            {
                Value = m_value - amount;
            }

            if (m_value <= 0 && OnDepleted != null)
                OnDepleted.Invoke();
        }
    }
}
