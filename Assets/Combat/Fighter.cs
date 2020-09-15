using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class Fighter : MonoBehaviour, CombatProperties.IKillable, CombatProperties.IDamagable<float>, CombatProperties.ICanAttack
    {
        public float MaxHealth = 100f;
        public float Health;

        public void Attack()
        {
            throw new System.NotImplementedException();
        }

        public void Die()
        {
            Debug.Log(name + " died", this);
            gameObject.SetActive(false);
        }

        public void TakeDamage(float damageTaken)
        {
            Health = Mathf.Clamp(Health - damageTaken, 0f, MaxHealth);
            if (Health == 0f) Die();
        }
    }
}
