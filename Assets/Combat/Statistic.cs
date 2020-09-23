using UnityEngine;

namespace Combat
{
    [System.Serializable]
    public class Statistic
    {
        public int max = 4;
        public int current;
        public void SetCurrent(int value)
        {
            current = value;
            UpdateVisual();
        }
        [Tooltip("The time it takes for a point to be recovered. 0 = no recovery.")]
        public float recoveryTime = 1f;
        [Tooltip("When enabled, this statistic is set to its maximum automatically on startup.")]
        public bool syncCurrentToMax = true;

        public GameObject Visualizer;
        public void UpdateVisual()
        {
            if (Visualizer == null)
            {
                Debug.LogError("Statistic visualizer is null");
                return;
            }
            //Debug.Log("Updating visual");
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
