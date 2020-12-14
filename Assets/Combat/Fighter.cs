using System.Collections.Generic;
using UnityEngine;
using BobJeltes;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Combat
{
    public class Fighter : MonoBehaviour, CombatProperties.IKillable, CombatProperties.IDamagable<int>, CombatProperties.ICanAttack
    {
        public Stat Health;
        public Stat Stamina;
        public StaminaRecharge staminaRecharge;
        private float InvincibilityTime = 0f;
        public bool Invincible
        {
            get
            {
                return InvincibilityTime > 0f;
            }
        }
        [System.Serializable]
        public class TouchDamage
        {
            public int damage = 0;
            public float invincibilityTime = 1f;
            public LayerMask layers;
            public float force = 1f;
        }
        public TouchDamage touchDamage;

        public List<GameObject> HitObjects = new List<GameObject>();
        [Tooltip("If disabled, every hit object in the list is spawned upon hit.")]
        public bool PickRandomHitObject;
        public List<GameObject> DeathObjects = new List<GameObject>();
        [Tooltip("If disabled, every death object in the list is spawned upon death.")]
        public bool PickRandomDeathObject;
        public List<GameObject> BloodSplatters = new List<GameObject>();
        [Tooltip("If enabled, only one bloodsplatter is instantiated upon death. If disabled, every bloodsplatter in the list is spawned upon death.")]
        public bool PickRandomBloodSplatter;

        public virtual void OnEnable()
        {
            OnEnableTasks();
        }

        internal void OnEnableTasks()
        {
            //Debug.Log("Set current health and stamina of " + name + " to max", this);
            if (Health != null)
            {
                Health.SetCurrent(Health.maxValue);
            }
            if (Stamina != null)
            {
                Stamina.SetCurrent(Stamina.maxValue);
            }
        }

        private void OnDisable()
        {
            OnDisableTasks();
        }

        internal void OnDisableTasks()
        {
            //Debug.Log(name + " disabled", this);
        }


        public virtual void Update()
        {
            ManageStaminaRecharge();
            ManageInvincibility();
        }

        void ManageStaminaRecharge()
        {
            if (!staminaRecharge.allow)
                return;
            if (Stamina.value < Stamina.maxValue)
            {
                if (staminaRecharge.windup < staminaRecharge.staminaRechargeWindupTime)
                {
                    staminaRecharge.windup += Time.deltaTime;

                }
                else
                {
                    if (staminaRecharge.recharge < staminaRecharge.staminaRechargeTime)
                    {
                        staminaRecharge.recharge += Time.deltaTime;
                    }
                    else
                    {
                        Stamina.SetCurrent(Stamina.value + 1);
                        staminaRecharge.recharge = 0f;
                    }
                }
            }
        }

        void ManageInvincibility()
        {
            if (Invincible)
            {
                InvincibilityTime -= Time.deltaTime;
            }
        }

        public void Attack()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Die()
        {
            Debug.Log(name + " died", this);
            if (PickRandomDeathObject)
            {
                SpawnObjectAtOwnHeight(DeathObjects[Random.Range(0, DeathObjects.Count)]);
            }
            else
            {
                foreach (GameObject obj in DeathObjects)
                {
                    SpawnObjectAtOwnHeight(obj);
                }
            }
            if (PickRandomBloodSplatter)
            {
                SpawnObjectAtOwnHeight(BloodSplatters[Random.Range(0, BloodSplatters.Count)]);
            }
            else
            {
                foreach (GameObject obj in BloodSplatters)
                {
                    SpawnObjectAtOwnHeight(obj);
                }
            }

            gameObject.SetActive(false);
        }

        public void SpawnObjectAtOwnHeight(GameObject prefab)
        {
            Instantiate(prefab, new Vector3(transform.position.x, prefab.transform.position.y, transform.position.z), prefab.transform.rotation).transform.localScale = transform.localScale;
        }

        public UnityEvent OnHitEvent;

        public void TakeDamage(int damageTaken)
        {
            Health.SetCurrent(Mathf.Clamp(Health.value - damageTaken, 0, Health.maxValue));

            OnHitEvent.Invoke();

            if (PickRandomHitObject)
            {
                Instantiate(HitObjects[Random.Range(0, HitObjects.Count)]);
            } else
            {
                foreach (GameObject obj in HitObjects)
                {
                    Instantiate(obj, new Vector3(transform.position.x, obj.transform.position.y, transform.position.z), obj.transform.rotation);
                }
            }
            //Debug.Log(name + " took damage", this);
            if (Health.value <= 0)
            {
                //Debug.Log(name + " should die now", this);
                Die();
            }
        }

        public void TakeDamage(int damageTaken, float invincibilityTime)
        {
            //Debug.Log("Take damage and remain invincible for " + invincibilityTime, this);
            InvincibilityTime = invincibilityTime;
            TakeDamage(damageTaken);
        }

        public void TakeDamage(int damageTaken, float invincibilityTime, Fighter damageSource)
        {
            Debug.Log(damageSource.name + " hit enemy " + name + " for " + damageTaken + " damage. New health: " + Health.value, this);
            TakeDamage(damageTaken, invincibilityTime);
            
        }

        private void OnCollisionEnter(Collision collision)
        {
            Fighter otherFighter = collision.gameObject.GetComponent<Fighter>();
            if (otherFighter == null)
            {
                return;
            }

            if (touchDamage.layers != (touchDamage.layers.value | (1 << otherFighter.gameObject.layer)))
            {
                return;
            }

            if (otherFighter.Invincible)
            {
                return;
            }

            if (touchDamage.damage != 0)
            {
                if (collision.rigidbody != null)
                {
                    Vector3 forceVector = collision.rigidbody.position - transform.position;
                    forceVector.Normalize();
                    collision.rigidbody.AddForce(touchDamage.force * forceVector, ForceMode.Impulse);
                }
                otherFighter.TakeDamage(touchDamage.damage, touchDamage.invincibilityTime, this);
                Debug.Log(otherFighter + " takes touch damage from " + name);
            }
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
