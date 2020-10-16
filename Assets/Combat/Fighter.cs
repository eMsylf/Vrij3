using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class Fighter : MonoBehaviour, CombatProperties.IKillable, CombatProperties.IDamagable<int>, CombatProperties.ICanAttack
    {
        public Statistic Health;
        public Statistic Stamina;
        public StaminaRecharge staminaRecharge;

        public int TouchDamage = 0;

        public List<GameObject> HitObjects = new List<GameObject>();
        public List<GameObject> DeathObjects = new List<GameObject>();

        private void OnEnable()
        {
            OnEnableTasks();
        }

        internal void OnEnableTasks()
        {
            Debug.Log("Set current health and stamina of " + name + " to max", this);
            if (Health.max != 0 && Health.syncCurrentToMax)
                Health.SetCurrent(Health.max);
            if (Stamina.max != 0 && Stamina.syncCurrentToMax)
                Stamina.SetCurrent(Stamina.max);

            Stamina.OnUse += () => staminaRecharge.windup = 0f;
            Stamina.OnUse += () => staminaRecharge.recharge = 0f;
        }

        private void OnDisable()
        {
            OnDisableTasks();
        }

        internal void OnDisableTasks()
        {
            Stamina.OnUse -= () => staminaRecharge.windup = 0f;
            Stamina.OnUse -= () => staminaRecharge.recharge = 0f;
        }


        private void Update()
        {
            ManageStaminaRecharge();
        }

        void ManageStaminaRecharge()
        {
            if (!staminaRecharge.allow)
                return;
            if (Stamina.Get() < Stamina.max)
            {
                if (staminaRecharge.windup < staminaRecharge.staminaRechargeWindupTime)
                {
                    staminaRecharge.windup += Time.unscaledDeltaTime;

                }
                else
                {
                    if (staminaRecharge.recharge < staminaRecharge.staminaRechargeTime)
                    {
                        staminaRecharge.recharge += Time.unscaledDeltaTime;
                    }
                    else
                    {
                        Stamina.SetCurrent(Stamina.current + 1);
                        staminaRecharge.recharge = 0f;
                    }
                }
            }
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

            foreach (GameObject obj in HitObjects)
            {
                Instantiate(obj, new Vector3(transform.position.x, obj.transform.position.y, transform.position.z), obj.transform.rotation);
                //Instantiate(obj);
            }
        }

        public void TakeDamage(int damageTaken, Fighter damageSource)
        {
            Debug.Log(damageSource.name + " hit enemy " + name + " for " + damageTaken + " damage. New health: " + Health.current, this);
            TakeDamage(damageTaken);
        }
        [System.Serializable]
        public class StaminaRecharge
        {
            [Tooltip("Time it takes for a stamina point to recover")]
            public float staminaRechargeTime = 1f;
            [Tooltip("Time between stamina usage and recovery")]
            public float staminaRechargeWindupTime = 1f;
            internal float recharge = 0f;
            internal float windup = 0f;
            public bool allow = true;
        }
    }
    
}
