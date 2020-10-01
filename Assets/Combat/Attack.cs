using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Combat {
    public class Attack : MonoBehaviour
    {
        public int Damage = 1;
        public bool CanMultiHit = false;

        public bool HitStun = true;
        [Min(0.001f)]
        public float HitStunSlowdown = .01f;
        [Min(0.001f)]
        public float HitStunDuration = .5f;

        public float ShakeDuration = .5f;
        public float ShakeStrength = .6f;

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
            Camera.main.DOShakePosition(ShakeDuration, ShakeStrength);
            fighterHit.TakeDamage(Damage);
            fightersHit.Add(fighterHit);
            TimeManager.Instance.DoSlowmotionWithDuration(HitStunSlowdown, HitStunDuration);
        }
    }
}