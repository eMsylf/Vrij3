using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class Fighter : MonoBehaviour, CombatProperties.IKillable, CombatProperties.IDamagable<int>, CombatProperties.ICanAttack
    {
        public Statistic Health;
        public Statistic Stamina;
        public int TouchDamage = 0;

        public List<GameObject> DeathObjects = new List<GameObject>();

        private void OnEnable()
        {
            EnableTasks();
        }

        internal void EnableTasks()
        {
            Debug.Log("Set current health and stamina of " + name + " to max", this);
            if (Health.max != 0 && Health.syncCurrentToMax)
                Health.SetCurrent(Health.max);
            if (Stamina.max != 0 && Stamina.syncCurrentToMax)
                Stamina.SetCurrent(Stamina.max);
        }

        public void Attack()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Die()
        {
            Debug.Log(name + " died", this);
            foreach (GameObject obj in DeathObjects)
            {
                Instantiate(obj, new Vector3(transform.position.x, obj.transform.position.y, transform.position.z), obj.transform.rotation);
                //Instantiate(obj);
            }
            gameObject.SetActive(false);
        }

        public void TakeDamage(int damageTaken)
        {
            Health.SetCurrent(Mathf.Clamp(Health.current - damageTaken, 0, Health.max));
            if (Health.current <= 0) Die();
        }

        public void TakeDamage(int damageTaken, Fighter damageSource)
        {
            Debug.Log(damageSource.name + " hit enemy " + name + " for " + damageTaken + " damage. New health: " + Health.current, this);
            TakeDamage(damageTaken);
        }

        public void UseStamina(int amount)
        {
            Stamina.SetCurrent(Mathf.Clamp(Stamina.current - amount, 0, Stamina.max));
        }
    }
}
