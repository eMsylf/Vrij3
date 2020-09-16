using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class Fighter : MonoBehaviour, CombatProperties.IKillable, CombatProperties.IDamagable<int>, CombatProperties.ICanAttack
    {
        public Statistic Health;
        public Statistic Stamina;
        public int TouchDamage = 0;

        private void OnEnable()
        {
            if (Health.syncCurrentToMax)
                Health.SetCurrent(Health.max);
            if (Stamina.syncCurrentToMax)
                Stamina.SetCurrent(Stamina.max);
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

        public void TakeDamage(int damageTaken, Fighter damageSource)
        {
            Debug.Log(damageSource.name + " hit enemy " + this.name + " for " + damageTaken + " damage. New health: " + this.Health.current, this);
            TakeDamage(damageTaken);
        }

        public void UseStamina(int amount)
        {
            Stamina.SetCurrent(Mathf.Clamp(Stamina.current - amount, 0, Stamina.max));
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
                Debug.Log("Updating visual");
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
}
