using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using BobJeltes.StandardUtilities;
using UnityEngine.Events;
using UnityEditor;
using RanchyRats.Gyrus;

namespace Gyrus.Combat
{
    public class Damager : MonoBehaviour
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
            public EDirection direction = EDirection.Forward;
            public ForceMode forceMode = ForceMode.VelocityChange;
            public float multiplier = 1f;
            [Tooltip("If ticked, a force away from the attack's pivot will be applied.")]
            public float outwardForceMultiplier = 1f;
            public Color debugColor = Color.white;
        }
        public AttackForce attackForce;

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
        // if (effect != Effect.MovementSpeedReduction
        [HideInInspector]
        [Range(0f, 1f)]
        public float MovementSpeedReduction = 1f;

        Character fighter;
        Character GetParentFighter()
        {
            if (fighter == null)
            {
                fighter = GetComponentInParent<Character>();
            }
            return fighter;
        }

        private List<Character> charactersHit = new List<Character>();

        public UnityEvent OnActivation;
        public UnityEvent OnDeactivation;

        private void OnEnable()
        {
            OnActivation.Invoke();
            charactersHit.Clear();
        }

        public void OnDisable()
        {
            OnDeactivation.Invoke();
        }

        public UnityEvent OnHitEvent;

        private void OnTriggerEnter(Collider other)
        {
            if (HitsTheseLayers != (HitsTheseLayers.value | 1 << other.gameObject.layer))
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
                Instantiate(obj, transform.position, Camera.main.transform.rotation);
            }

            OnHitEvent.Invoke();

            Character characterHit = other.GetComponent<Character>();
            if (characterHit == null)
                characterHit = other.GetComponentInParent<Character>();
            Character parentCharacter = GetParentFighter();
            Debug.Log("Hit fighter: " + characterHit, gameObject);

            if (other.attachedRigidbody != null && !other.attachedRigidbody.isKinematic)
            {
                AddAttackForceTo(other.attachedRigidbody);
            }

            if (characterHit == null)
                return;

            if (parentCharacter == characterHit)
            {
                Debug.Log("Hit self", parentCharacter);
                return;
            }

            if (characterHit.Invincible)
            {
                //Debug.Log("<color=yellow>Couldn't hit fighter because of invincibility</color>");
                return;
            }

            if (charactersHit.Contains(characterHit))
            {
                Debug.Log(name + " tried to multihit " + characterHit.name, this);
                if (!CanMultiHit)
                {
                    return;
                }
            }

            // The hit is succesful

            charactersHit.Add(characterHit);

            switch (effect)
            {
                case Effect.Health:
                    //Debug.Log("Deal " + Damage + " health damage");
                    DamageCharacter(parentCharacter, characterHit);
                    break;
                case Effect.Stamina:
                    Debug.Log("Drain " + StaminaReduction + " stamina");
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

        public void AddAttackForceTo(Rigidbody rigidbody)
        {
            Vector3 forceVector = new Vector3();

            if (attackForce.outwardForceMultiplier != 0f)
                forceVector += (rigidbody.position - transform.position) * attackForce.outwardForceMultiplier;
            if (attackForce.multiplier != 0f)
                forceVector += GetForceVector(attackForce.direction);

            rigidbody.AddForce(forceVector, attackForce.forceMode);
        }

        public void DamageCharacter(Character attacker, Character victim)
        {

            if (attacker != null)
            {
                victim.TakeDamage(Damage, InvincibilityTime, attacker);
            }
            else
            {
                victim.TakeDamage(Damage, InvincibilityTime);
            }
        }

        Vector3 GetForceVector(AttackForce.EDirection direction)
        {
            Vector3 returnForce = new Vector3();
            switch (direction)
            {
                case AttackForce.EDirection.Forward:
                    returnForce = transform.forward * attackForce.multiplier;
                    break;
                case AttackForce.EDirection.Right:
                    returnForce = transform.right * attackForce.multiplier;
                    break;
                case AttackForce.EDirection.Up:
                    returnForce = transform.up * attackForce.multiplier;
                    break;
            }
            return returnForce;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (attackForce.multiplier != 0f)
            {
                Handles.color = attackForce.debugColor;
                Vector3 forceVector = GetForceVector(attackForce.direction);

                Vector3 toPosition = transform.position + forceVector;
                Handles.DrawLine(transform.position, toPosition);

                Vector3 lookDir = forceVector * (attackForce.multiplier >= 0f ? 1f : -1f);
                Handles.ArrowHandleCap(0, toPosition, Quaternion.LookRotation(lookDir), attackForce.multiplier, EventType.Repaint);

            }
            if (attackForce.outwardForceMultiplier != 0f)
            {
                Gizmos.color = attackForce.debugColor;
                Gizmos.DrawWireSphere(transform.position, attackForce.outwardForceMultiplier);
            }
        }
#endif
    }
}