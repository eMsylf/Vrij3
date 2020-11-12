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
        [System.Serializable]
        public class AttackForce
        {
            public enum EDirection
            {
                Forward,
                Right,
                Up
            }
            public EDirection Direction = EDirection.Forward;
            public float Multiplier;
            [Tooltip("If ticked, physics objects will be hit away from the attack's pivot instead of just in the direction specified.")]
            public bool AwayFromSelf;
        }
        public AttackForce attackForce;

        public enum Effect
        {
            Health,
            Stamina,
            ChargeSpeed,
            DisableCharge,
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
        public float chargeSpeedReduction = 1f;
        // if (effect != Effect.MovementSpeedReduction
            [HideInInspector]
        [Range(0f, 1f)]
        public float MovementSpeedReduction = 1f;

        Fighter fighter;
        Fighter GetParentFighter()
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

            //Debug.Log(name + " hit " + other.name, this);
            //Debug.DrawLine(transform.position, other.transform.position, Color.white, 2f);

            // Hit something the attack can hit
            foreach (GameObject obj in HitEffects)
            {
                Instantiate(obj, other.transform.position, Camera.main.transform.rotation);
            }

            Fighter fighterHit = other.attachedRigidbody?.GetComponent<Fighter>();
            Fighter parentFighter = GetParentFighter();
            
            if (other.attachedRigidbody != null)
            {
                if (attackForce.AwayFromSelf)
                    other.attachedRigidbody.AddForce((other.transform.position - transform.position) + GetForceVector(attackForce.Direction), ForceMode.Impulse);
                else
                    other.attachedRigidbody.AddForce(GetForceVector(attackForce.Direction), ForceMode.Impulse);
            }

            if (fighterHit == null)
                return;

            if (parentFighter == fighterHit)
            {
                Debug.Log("Hit self", parentFighter);
                return;
            }

            if (fighterHit.Invincible)
            {
                //Debug.Log("<color=yellow>Couldn't hit fighter because of invincibility</color>");
                return;
            }

            if (fightersHit.Contains(fighterHit))
            {
                Debug.Log(name + " tried to multihit" + fighterHit.name, this);
                if (!CanMultiHit)
                {
                    return;
                }
            }

            // The hit is succesful

            fightersHit.Add(fighterHit);


            switch (effect)
            {
                case Effect.Health:
                    Debug.Log("Deal " + Damage + " health damage");
                    DamageFighter(parentFighter, fighterHit);
                    break;
                case Effect.Stamina:
                    Debug.Log("Drain " + StaminaReduction + " stamina");
                    break;
                case Effect.ChargeSpeed:
                    Debug.Log("Reduce charge speed by " + chargeSpeedReduction);
                    break;
                case Effect.DisableCharge:
                    Debug.Log("Disable charge");
                    if (fighterHit is PlayerController)
                    {
                        fighterHit.GetComponent<PlayerController>().attacking.AllowCharging(false);
                    }
                    break;
                case Effect.MovementSpeed:
                    Debug.Log("Reduce movement speed by " + MovementSpeedReduction);
                    break;
            }
            
            if (CameraShake.enabled)
                Camera.main.DOShakePosition(CameraShake.Duration, CameraShake.Strength);
            if (HitStun.enabled)
                TimeManager.Instance.DoSlowmotionWithDuration(HitStun.Slowdown, HitStun.Duration);
        }

        public void DamageFighter(Fighter attacker, Fighter damaged)
        {

            if (attacker != null)
            {
                damaged.TakeDamage(Damage, InvincibilityTime, damaged);
            }
            else
            {
                damaged.TakeDamage(Damage, InvincibilityTime);
            }
        }

        Vector3 GetForceVector(AttackForce.EDirection direction)
        {
            Vector3 returnForce = new Vector3();
            switch (direction)
            {
                case AttackForce.EDirection.Forward:
                    returnForce = transform.forward * attackForce.Multiplier;
                    break;
                case AttackForce.EDirection.Right:
                    returnForce = transform.right * attackForce.Multiplier;
                    break;
                case AttackForce.EDirection.Up:
                    returnForce = transform.up * attackForce.Multiplier;
                    break;
            }
            return returnForce;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(transform.position, transform.position + GetForceVector(attackForce.Direction));
            Gizmos.DrawWireSphere(transform.position + GetForceVector(attackForce.Direction), 1f);
        }
#endif
    }
}