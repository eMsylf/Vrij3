using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Combat {
    public class Attack : MonoBehaviour
    {
        public bool CanMultiHit = false;
        [Min(0f)]
        public float InvincibilityTime = 0f;

        public bool HitStun = true;
        [Min(0.001f)]
        public float HitStunSlowdown = .01f;
        [Min(0.001f)]
        public float HitStunDuration = .5f;

        public float ShakeDuration = .5f;
        public float ShakeStrength = .6f;

        public enum Effect
        {
            Health,
            Stamina,
            ChargeSpeed,
            MovementSpeed
        }
        public Effect effect = Effect.Health;
        // if (effect != Effect.Health)
            [HideInInspector]
        public int Damage = 1;
        // if (effect != Effect.Stamina)
            [HideInInspector]
        public int StaminaReduction = 1;
        // if (effect != Effect.ChargeReduction)
            [HideInInspector]
        [Range(0f, 1f)]
        public float ChargeReduction = 1;
        // if (effect != Effect.MovementSpeedReduction
            [HideInInspector]
        [Range(0f, 1f)]
        public float MovementSpeedReduction = 1;

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
            Fighter fighterHit = other.attachedRigidbody?.GetComponent<Fighter>();
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
            fighterHit.TakeDamage(Damage, InvincibilityTime);
            fightersHit.Add(fighterHit);
            TimeManager.Instance.DoSlowmotionWithDuration(HitStunSlowdown, HitStunDuration);
        }
    }
}