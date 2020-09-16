using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class Fighter : MonoBehaviour, CombatProperties.IKillable, CombatProperties.IDamagable<int>, CombatProperties.ICanAttack
    {
        public Statistic Health;
        public Statistic Stamina;

        private void OnEnable()
        {
            if (!Health.syncCurrentToMax)
                Health.current = Health.max;
            if (!Stamina.syncCurrentToMax)
                Stamina.current = Stamina.max;
        }

        public void Attack()
        {
            throw new System.NotImplementedException();
        }

        public void Die()
        {
            Debug.Log(name + " died", this);
            gameObject.SetActive(false);
        }

        public void TakeDamage(int damageTaken)
        {
            Health.SetCurrent(Mathf.Clamp(Health.current - damageTaken, 0, Health.max));
            if (Health.current <= 0) Die();
        }

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
            [Tooltip("When enabled, this statistic does not get set to its maximum automatically on startup.")]
            public bool syncCurrentToMax;

            public GameObject Visualizer;
            public void UpdateVisual()
            {
                if (Visualizer == null)
                {
                    Debug.LogError("Statistic visualizer is null");
                    return;
                }
                Debug.Log("Updating visual");
                for (int i = 0; i < Visualizer.transform.childCount; i++)
                {
                    GameObject child = Visualizer.transform.GetChild(i).gameObject;
                    bool shouldBeActive = current >= i + 1;
                    child.SetActive(shouldBeActive);
                    Debug.Log("This child (" + i + ") should be active: " + shouldBeActive);
                }
            }
        }
    }
}
