using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BobJeltes
{
    public class SliderExtension : MonoBehaviour
    {
        [HideInInspector]
        public float maxValue = 1;
        [HideInInspector]
        public float value;
        public bool invertValue;
        public float InvertedValue
        {
            get => maxValue - value;
        }

        //==========================================

        public FloatEvent OnValueSet;
        public FloatEvent OnMaxSet;

        public virtual void SetValue(float newValue)
        {
            value = newValue;
            Debug.Log("Update value: " + newValue);
            OnValueSet.Invoke(invertValue?InvertedValue:value);
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

            value = newMax * (value / oldMax);


            OnMaxSet.Invoke(maxValue);
            SetValue(value);
        }
    }

    [System.Serializable]
    public class FloatEvent : UnityEvent<float>
    {

    }
}
