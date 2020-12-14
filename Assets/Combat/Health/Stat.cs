using System;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    [System.Serializable]
    public class Stat : MonoBehaviour
    {
        public int maxValue = 4;
        public int value;
        [Tooltip("The time it takes for a point to be recovered. 0 = no recovery.")]
        public float recoveryTime = 1f;
        [Tooltip("When enabled, this statistic is set to its maximum automatically on startup.")]
        public bool syncCurrentToMax = true;
        public GameObject Visualizer;

        public UnityAction<float> OnUse;
        public UnityAction OnDepleted;

        public int Get()
        {
            return value;
        }

        public void SetCurrent(int _value)
        {
            value = _value;
        }

        public bool AttemptUse()
        {
            return Use(1);
        }

        public bool Use(int amount)
        {
            if (value - amount >= 0)
            {
                SetCurrent(value - amount);
                OnUse.Invoke(amount);
                return true;
            }
            if (OnDepleted != null)
                OnDepleted.Invoke();
            return false;
        }

        public void UpdateVisual(bool animate)
        {
            if (Visualizer == null)
            {
                //Debug.LogError("Statistic visualizer is null");
                return;
            }

            Visualizer.SetActive(true);

            if (animate)
            {
                Animation animComponent = Visualizer.GetComponent<Animation>();
                if (animComponent != null)
                {
                    animComponent.Play();
                }
            }

            FadeOut fadeOutComponent = Visualizer.GetComponent<FadeOut>();
            if (fadeOutComponent != null)
            {
                fadeOutComponent.gameObject.SetActive(true);
                fadeOutComponent.ResetFade();
                fadeOutComponent.StartFadeOut();
            }

            //Debug.Log("Updating visual", Visualizer);
            for (int i = 0; i < Visualizer.transform.childCount; i++)
            {
                GameObject child = Visualizer.transform.GetChild(i).gameObject;
                bool shouldBeActive = value >= i + 1;
                child.SetActive(shouldBeActive);
                //Debug.Log("This child (" + i + ") should be active: " + shouldBeActive);
            }
        }
    }

}
