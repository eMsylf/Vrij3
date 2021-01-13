using UnityEngine;
using UnityEngine.Events;

namespace BobJeltes
{
    public class SliderExtension : MonoBehaviour
    {
        [HideInInspector]
        public float maxValue = 1;
        [HideInInspector]
        public float m_value;
        public bool invertValue;
        public float Value
        {
            get => invertValue ? InvertedValue : m_value;
            set => m_value = value;
        }
        public float InvertedValue
        {
            get => maxValue - m_value;
        }

        //==========================================

        public FloatEvent OnValueSet;
        public FloatEvent OnMaxSet;

        public virtual void SetValue(float newValue)
        {
            Value = newValue;
            //Debug.Log("Update value: " + newValue, gameObject);
            OnValueSet.Invoke(Value);
        }

        public void SetMax(float newMax)
        {
            float oldMax = maxValue;
            maxValue = newMax;
            //Debug.Log("Update max: " + Max);

            float difference = newMax - oldMax;
            if (difference > 0)
            {
                // Maximum omhoog
            }
            else if (difference < 0)
            {
                // Maximum omlaag
            }

            Value = newMax * (Value / oldMax);


            OnMaxSet.Invoke(maxValue);
            SetValue(Value);
        }
    }

    [System.Serializable]
    public class FloatEvent : UnityEvent<float>
    {

    }
}
