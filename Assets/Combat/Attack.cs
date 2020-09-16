using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat {
    public class Attack : MonoBehaviour
    {
        public int Damage = 1;

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
            Fighter fighterHit = other.gameObject.GetComponent<Fighter>();
            Fighter parent = GetComponentInParent<Fighter>();

            if (fighterHit == null)
                return;

            if (parent == fighterHit)
            {
                return;
            }

            fighterHit.TakeDamage(Damage);
            Debug.Log(GetFighter().name + " hit enemy " + fighterHit.name + " for " + Damage + " damage. New health: " + fighterHit.Health.current , fighterHit);
        }
    }
}