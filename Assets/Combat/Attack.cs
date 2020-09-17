using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat {
    public class Attack : MonoBehaviour
    {
        public int Damage = 1;
        public bool CanMultiHit = false;

        Fighter fighter;
        Fighter GetFighter()
        {
            if (fighter == null)
            {
                fighter = GetComponentInParent<Fighter>();
            }
            return fighter;
        }

        private List<Fighter> fightersHit = new List<Fighter>();

        private void OnEnable()
        {
            fightersHit.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            Fighter fighterHit = other.attachedRigidbody.GetComponent<Fighter>();
            Fighter parent = GetComponentInParent<Fighter>();

            if (fighterHit == null)
                return;

            if (parent == fighterHit)
            {
                Debug.Log("Hit self", parent);
                return;
            }

            if (fightersHit.Contains(fighterHit))
            {
                //Debug.Log("Already hit this fighter");
                if (!CanMultiHit)
                {
                    //Debug.Log("Trying to multihit but is disabled", this);
                    return;
                }
            }

            fighterHit.TakeDamage(Damage);
            fightersHit.Add(fighterHit);
        }
    }
}