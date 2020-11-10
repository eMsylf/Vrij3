using System;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    [System.Serializable]
    public class Statistic
    {
        public int max = 4;
        public int current;
        [Tooltip("The time it takes for a point to be recovered. 0 = no recovery.")]
        public float recoveryTime = 1f;
        [Tooltip("When enabled, this statistic is set to its maximum automatically on startup.")]
        public bool syncCurrentToMax = true;
        public GameObject Visualizer;

        public UnityAction OnUse;

        public int Get()
        {
            return current;
        }

        public void SetCurrent(int value)
        {
            SetCurrent(value, true);
        }

        public void SetCurrent(int value, bool updateVisual)
        {
            current = value;
            if (updateVisual) 
                UpdateVisual();
        }

        public bool AttemptUse()
        {
            return Use(1);
        }

        public bool Use(int amount)
        {
            if (current - amount >= 0)
            {
                SetCurrent(current - amount);
                OnUse.Invoke();
                return true;
            }
            return false;
        }

        public void UpdateVisual()
        {
            if (Visualizer == null)
            {
                Debug.LogError("Statistic visualizer is null");
                return;
            }

            Animation animComponent = Visualizer.GetComponent<Animation>();
            if (animComponent != null)
            {
                animComponent.Play();
            }

            FadeOut fadeOutComponent = Visualizer.GetComponent<FadeOut>();
            if (fadeOutComponent != null)
            {
                fadeOutComponent.ResetFade();
                fadeOutComponent.StartFadeOut();
            }

            Debug.Log("Updating visual", Visualizer);
            for (int i = 0; i < Visualizer.transform.childCount; i++)
            {
                GameObject child = Visualizer.transform.GetChild(i).gameObject;
                bool shouldBeActive = current >= i + 1;
                child.SetActive(shouldBeActive);
                //Debug.Log("This child (" + i + ") should be active: " + shouldBeActive);
            }
        }
    }
    
}
