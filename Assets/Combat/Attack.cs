using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using BobJeltes.StandardUtilities;

namespace Combat {
    public class Attack : MonoBehaviour
    {
        public bool CanMultiHit = false;
        public LayerMask HitsTheseLayers;
        
        [Min(0f)]
        public float InvincibilityTime = 0f;

        public HitStunSettings HitStun;

        public CameraShakeSettings CameraShake;
        public List<GameObject> HitEffects = new List<GameObject>();

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
                //Debug.DrawLine(transform.position, other.transform.position, Color.red, 2f);
                return;
            }

            // Hit something the attack can hit
            foreach (GameObject obj in HitEffects)
            {
                Instantiate(obj, other.transform.position, Camera.main.transform.rotation);
            }

            //Debug.Log(name + " hit " + other.name, this);
            //Debug.DrawLine(transform.position, other.transform.position, Color.white, 2f);

            Fighter fighterHit = other.attachedRigidbody?.GetComponent<Fighter>();
            Fighter parentFighter = GetComponentInParent<Fighter>();
            
            
            if (other.attachedRigidbody != null)
            {
                if (AwayFromSelf)
                    other.attachedRigidbody.AddForce((other.transform.position - transform.position) + GetForceVector(Direction), ForceMode.Impulse);
                else
                    other.attachedRigidbody.AddForce(GetForceVector(Direction), ForceMode.Impulse);
            }

            if (fighterHit == null)
                return;

            if (parentFighter == fighterHit)
            {
                Debug.Log("Hit self", parentFighter);
                return;
            }

            if (fighterHit.Invincible)
                return;

            if (fightersHit.Contains(fighterHit))
            {
                Debug.Log(name + " tried to multihit" + fighterHit.name, this);
                if (!CanMultiHit)
                {
                    return;
                }
            }

            fightersHit.Add(fighterHit);
            
            // The hit is succesful

            if (parentFighter != null)
            {
                fighterHit.TakeDamage(Damage, InvincibilityTime, parentFighter);
            }
            else
            {
                fighterHit.TakeDamage(Damage, InvincibilityTime);
            }
            
            if (CameraShake.enabled)
                Camera.main.DOShakePosition(CameraShake.Duration, CameraShake.Strength);
            if (HitStun.enabled)
                TimeManager.Instance.DoSlowmotionWithDuration(HitStun.Slowdown, HitStun.Duration);
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