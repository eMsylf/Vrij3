using BobJeltes;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gyrus
{
    public class CharacterStatistic : MonoBehaviour
    {
        [SerializeField]
        [Min(0)]
        private float m_maxValue = 4;
        public float MaxValue
        {
            get => m_maxValue;
            set => m_maxValue = value;
        }
        [SerializeField]
        private float m_value;
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
                    events.OnValueIncrease.Invoke();
                }
                else
                {
                    if (!AllowUnderflow && value < 0)
                        value = 0;
                    events.OnValueDecrease.Invoke();
                }

                m_value = value;
                events.OnValueChanged.Invoke(value);
                UpdateVisual(value, true);
            }
        }
        /// <summary>
        /// A representation of the value between 0-1, where 0 is 0, and 1 is the MaxValue
        /// </summary>
        public float ValueRelative
        {
            set => Value = Mathf.Lerp(0, MaxValue, value);
            get => Mathf.InverseLerp(0, MaxValue, Value);
        }

        public bool AllowOverflow;
        public bool AllowUnderflow;
        [Tooltip("When enabled, this statistic is set to its maximum automatically when Start() is called.")]
        public bool setValueToMax = true;
        public Events events = new Events();

        [Header("Visualization")]
        [Tooltip("This visualizer turns child transforms on and off based on the statistic's value")]
        public Transform TransformwiseVisualizer;
        public Slider SliderVisualizer;

        [Header("Recharge")]
        public bool allowRecharge = false;
        [Min(0)]
        [Tooltip("The time it takes for the recharge time to start counting, after using this value")]
        public float rechargeWindupTime = 1f;
        [Min(0)]
        [Tooltip("The time it takes for a point to be recharged")]
        public float rechargeTime = 1f;
        [Min(0)]
        public float rechargeAmount = 1f;
        private float windupRemaining;
        private float timeBeforeRecharge;

        public void SetValueWithoutEvent(float value)
        {
            if (m_value == value)
                return;

            if (value > m_value && !AllowOverflow && value > MaxValue)
                value = MaxValue;
            else if (!AllowUnderflow && value < 0)
                value = 0;

            m_value = value;
            UpdateVisual(value, true);
        }
        [Serializable]
        public struct Events
        {
            public FloatEvent OnValueChanged;
            public UnityEvent OnValueIncrease;
            public UnityEvent OnValueDecrease;
            public UnityEvent OnDepleted;
        }

        private void Start()
        {
            if (setValueToMax)
            {
                Value = MaxValue;
            }
            else
                UpdateVisual(Value, false);
        }

        private void Update()
        {
            ManageRecharge();
        }

        void ManageRecharge()
        {
            if (!allowRecharge)
                return;
            if (Value >= MaxValue)
                return;
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
            Value += rechargeAmount;
            timeBeforeRecharge = rechargeTime;
        }

        public bool IsEmpty(bool fireEvent = true)
        {
            if (fireEvent) events.OnDepleted.Invoke();
            return Value <= 0;
        }

        public void Use(float amount)
        {
            if (Value - amount >= 0)
                Value -= amount;

            if (Value <= 0 && events.OnDepleted != null)
                events.OnDepleted.Invoke();
        }

        public void UpdateVisual(float value, bool animate)
        {
            if (TransformwiseVisualizer == null)
            {
                //Debug.LogError("Statistic visualizer is null");
                return;
            }

            if (animate)
            {
                Animation animComponent = TransformwiseVisualizer.GetComponent<Animation>();
                if (animComponent != null)
                    animComponent.Play();
            }

            FadeOut fadeOutComponent = TransformwiseVisualizer.GetComponent<FadeOut>();
            if (fadeOutComponent != null)
            {
                fadeOutComponent.gameObject.SetActive(true);
                fadeOutComponent.ResetFade();
                fadeOutComponent.StartFadeOut();
            }

            for (int i = 0; i < TransformwiseVisualizer.childCount; i++)
            {
                GameObject child = TransformwiseVisualizer.transform.GetChild(i).gameObject;
                bool shouldBeActive = value >= i + 1;
                child.SetActive(shouldBeActive);
            }
        }
    }
}
