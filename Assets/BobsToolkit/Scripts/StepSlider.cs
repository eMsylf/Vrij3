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
        public int Max = 1;
        [HideInInspector]
        public int Current;
        public float Ratio
        {
            get
            {
                return Current / Max;
            }
        }

        //==========================================

        public IntEvent OnValueUpdate;
        public IntEvent OnMaxChanged;

        public void UpdateCurrent()
        {
            Debug.Log("Update value: " + Current);
            OnValueUpdate.Invoke(Current);
        }

        public void UpdateMax(int newMax)
        {
            int oldMax = Max;
            Max = newMax;
            //Debug.Log("Update max: " + Max);

            int difference = newMax - oldMax;
            if (difference > 0)
            {
                // Maximum omhoog
            }
            else if (difference < 0)
            {

            }

            int newCurrent = Current * (newMax / oldMax);
            Current = newCurrent;
            OnMaxChanged.Invoke(Max);
            UpdateCurrent();
        }
    }

    [System.Serializable]
    public class IntEvent : UnityEvent<int>
    {

    }
}
