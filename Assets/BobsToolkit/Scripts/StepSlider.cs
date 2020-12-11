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
        public bool invertCurrent;

        //==========================================

        public IntEvent OnValueUpdate;
        public IntEvent OnMaxChanged;

        public void UpdateCurrent(int newCurrent)
        {
            int hiddenCurrent;
            if (invertCurrent)
                hiddenCurrent = Max - newCurrent;
            else
                hiddenCurrent = Current;
            //Debug.Log("Update value: " + Current);
            Current = newCurrent;
            OnValueUpdate.Invoke(hiddenCurrent);
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
                // Maximum omlaag
            }

            Current = Mathf.RoundToInt(newMax * (Current / (float)oldMax));


            OnMaxChanged.Invoke(Max);
            UpdateCurrent(Current);
        }
    }

    [System.Serializable]
    public class IntEvent : UnityEvent<int>
    {

    }
}
