using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat {
    public class Attack : MonoBehaviour
    {
        public float Damage = 1f;

        Fighter fighter;
        Fighter GetFighter()
        {
            if (fighter == null)
            {
                fighter = GetComponentInParent<Fighter>();
            }
            return fighter;
        }

        private void OnTriggerEnter(Collider other)
        {
            Enemy enemyHit = other.gameObject.GetComponent<Enemy>();
            if (enemyHit != null)
            {
                enemyHit.TakeDamage(Damage);
                Debug.Log(GetFighter().name + " hit enemy " + enemyHit.name + " for " + Damage + " damage. New health: " + enemyHit.Health , enemyHit);
            }
        }
    }
}