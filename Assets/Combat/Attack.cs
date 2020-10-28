using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Combat {
    public class Attack : MonoBehaviour
    {
        public bool CanMultiHit = false;
        public LayerMask HitsTheseLayers;
        
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
        public float Force;
        public enum EDirection
        {
            Forward,
            Right,
            Up
        }
        public EDirection Direction = EDirection.Forward;
        [Tooltip("If ticked, physics objects will be hit away from the attack's pivot instead of just in the direction specified.")]
        public bool AwayFromSelf;

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
            if (HitsTheseLayers != (HitsTheseLayers.value | (1 << other.gameObject.layer)))
            {
                //Debug.Log(name + " hit " + other.name + " on ignored layer: " + other.gameObject.layer, this);
                return;
            }

            Fighter fighterHit = other.attachedRigidbody?.GetComponent<Fighter>();
            Fighter parent = GetComponentInParent<Fighter>();
            
            
            if (other.attachedRigidbody != null)
            {
                //other.attachedRigidbody.AddForce(Vector3.Scale(other.transform.position - transform.position, GetForceVector(Direction)), ForceMode.Impulse);
                if (AwayFromSelf)
                    other.attachedRigidbody.AddForce((other.transform.position - transform.position) + GetForceVector(Direction), ForceMode.Impulse);
                else
                    other.attachedRigidbody.AddForce(GetForceVector(Direction), ForceMode.Impulse);
                //other.attachedRigidbody.AddForce(other.transform.position - transform.position, ForceMode.Impulse);
            }

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
            else
            {
                fightersHit.Add(fighterHit);
            }
            Camera.main.DOShakePosition(ShakeDuration, ShakeStrength);
            fighterHit.TakeDamage(Damage, InvincibilityTime);
            TimeManager.Instance.DoSlowmotionWithDuration(HitStunSlowdown, HitStunDuration);
        }

        Vector3 GetForceVector(EDirection direction)
        {
            Vector3 returnForce = new Vector3();
            switch (direction)
            {
                case EDirection.Forward:
                    returnForce = transform.forward * Force;
                    break;
                case EDirection.Right:
                    returnForce = transform.right * Force;
                    break;
                case EDirection.Up:
                    returnForce = transform.up * Force;
                    break;
            }
            return returnForce;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(transform.position, transform.position + GetForceVector(Direction));
            Gizmos.DrawWireSphere(transform.position + GetForceVector(Direction), 1f);
        }
    }
#endif
}