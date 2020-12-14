using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BobJeltes
{
    public class StepSlider : MonoBehaviour
    {
        [HideInInspector]
        public int maxValue = 1;
        [HideInInspector]
        public int value;
        public bool invertValue;
        public int InvertedValue
        {
            get => maxValue - value;
        }

        //==========================================

        public IntEvent OnValueUpdate;
        public IntEvent OnMaxChanged;

        public void SetCurrent(int newCurrent)
        {
            value = newCurrent;
            //Debug.Log("Update value: " + Current);
            OnValueUpdate.Invoke(invertValue?InvertedValue:value);
        }

        public void SetMax(int newMax)
        {
            int oldMax = maxValue;
            maxValue = newMax;
            //Debug.Log("Update max: " + Max);

            int difference = newMax - oldMax;
            if (difference > 0)
            {
                // Maximum omhoog
            }
            else if (difference < 0)
            {
                // Maximum omlaag
            }

            value = Mathf.RoundToInt(newMax * (value / (float)oldMax));


            OnMaxChanged.Invoke(maxValue);
            SetCurrent(value);
        }
    }

    [System.Serializable]
    public class IntEvent : UnityEvent<int>
    {

    }
}
