using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using BobJeltes.Attributes;
#if UNITY_EDITOR
#endif
using Combat;

namespace RanchyRats.Gyrus
{
    public class Attacking : CharacterComponent
    {
        [System.Serializable]
        public struct Sounds
        {
            public FMODUnity.StudioEventEmitter attackChargeSound;
            public FMODUnity.StudioEventEmitter attackChargeTick1Sound;
            public FMODUnity.StudioEventEmitter attackChargeTick2Sound;
            public FMODUnity.StudioEventEmitter attackChargeTick3Sound;
            public FMODUnity.StudioEventEmitter attackChargeTick4Sound;

            public FMODUnity.StudioEventEmitter attack1Sound;
            public FMODUnity.StudioEventEmitter attack2Sound;
            public FMODUnity.StudioEventEmitter attack3Sound;

            public void PlayAttackSound(int index)
            {
                if (index == 0) attack1Sound.Play();
                else if (index == 1) attack2Sound.Play();
                else if (index == 2) attack3Sound.Play();
            }
        }
        public Sounds sounds;

        [Header("Charging")]
        public Slider ChargeSlider;
        public Gradient ChargeZones;
        public Statistic ChargeIndicator;
        [Tooltip("Time it takes for the slider to fill up")]
        [Min(0)]
        public float ChargeTime = 2f;
        [Tooltip("Time below which a charge will not be initiated")]
        [Min(0)]
        public float ChargeDeadzone = .1f;
        [System.Serializable]
        public struct EnergyCost
        {
            public EnergyAbsorption energyAbsorption;
            [Min(0)]
            public int fullChargeCost;
            [Range(0f, 1f)]
            public float EnergyRequirementTrigger;
            internal bool CanFullCharge
            {
                get
                {
                    if (energyAbsorption == null) return true;
                    return energyAbsorption.Energy >= fullChargeCost;
                }
            }
        }
        public EnergyCost energyCost = new EnergyCost();

        [Tooltip("If true, the charge will slow down like everything else")]
        public bool SlowmotionAffectsCharge = false;

        [Range(0f, 1f)]
        public float slowmotionTrigger = .5f;
        [Range(0f, 1f)]
        public float slowmotionFactor = .25f;

        internal bool slowmotionStarted = false;

        public bool AllowCharging = true;
        public bool IsCharging = false;
        public GameObject ChargeBlockedIndicator;
        private float latestCharge = 0f;

        [Header("Attacks")]
        [Tooltip("If true, uses the SingleWeaponObject. Otherwise, uses the attacks from the list.")]
        public bool SingleWeaponObject = true;
        public Animator WeaponAnimator;
        public Attack[] attacks = new Attack[0];
        [System.Serializable]
        public struct Attack
        {
            public int staminaCost;
            public Restrictions restrictions;
            public Damager damageObject;
        }
        public Attack GetAttack(int index) => (index > attacks.Length - 1) ? attacks[attacks.Length - 1] : attacks[index];

        [Flags] public enum Restrictions
        {
            None = 0,
            Move = 1,
            Rotate = 2,
            StaminaRecovery = 4
        }

        public UnityEvent OnAttackAnnouncement;
        public UnityEvent OnAttackStarted;
        public UnityEvent OnAttackEnded;

        public enum State
        {
            Ready,
            Charging,
            Attacking,
            OnCooldown,
            Disabled
        }

        private void OnEnable()
        {
            ClearChargeBlockers();
            // Deactivate all damage objects and add a callback to EndAttack for when the object is set to inactive next time
            for (int i = 0; i < attacks.Length; i++)
            {
                if (attacks[i].damageObject == null) continue;
                attacks[i].damageObject.gameObject.SetActive(false);
                attacks[i].damageObject.OnDeactivation.AddListener(EndAttack);
            }
            
        }

        private void OnDisable()
        {
            EndCharge(false);
            for (int i = 0; i < attacks.Length; i++)
            {
                if (attacks[i].damageObject == null) continue;
                attacks[i].damageObject.OnDeactivation.RemoveListener(EndAttack);
            }
        }

        int GetChargeIndex(float time)
        {
            for (int i = 0; i < ChargeZones.colorKeys.Length; i++)
            {
                if (ChargeZones.colorKeys[i].time >= time)
                {
                    //Debug.Log("Time: " + time + " index " + i);
                    return i;
                }
            }
            Debug.Log("No index at time " + time + " found. Returning max: " + (ChargeZones.colorKeys.Length - 1));
            return ChargeZones.colorKeys.Length - 1;
        }

        [Tooltip("The number of triggers that the player is inside of, prohibiting its charge")]
        internal List<GameObject> chargeBlockers = new List<GameObject>();

        public void AddChargingBlocker(GameObject blocker)
        {
            chargeBlockers.Add(blocker);
            ChargeBlockedIndicator.SetActive(chargeBlockers.Count != 0);
        }

        public void RemoveChargingBlocker(GameObject blocker)
        {
            chargeBlockers.Remove(blocker);
            ChargeBlockedIndicator.SetActive(chargeBlockers.Count != 0);
        }

        internal void ClearChargeBlockers()
        {
            chargeBlockers.Clear();
            if (ChargeBlockedIndicator != null)
                ChargeBlockedIndicator.SetActive(false);
        }

        internal bool ChargingAllowed()
        {
            return chargeBlockers.Count == 0;
        }

        public void EndCharge(bool complete)
        {
            //if (state != State.Charging)
            //{
            //    Debug.Log("No charge to complete");
            //    return;
            //}

            if (complete)
            {
                StartAttack(GetChargeIndex(latestCharge));
            }
            else
            {
                StopCoroutine(DoCharge());
                ChargeIndicator.SetCurrent(0);
                if (slowmotionStarted) TimeManager.Instance.StopSlowmotion();
            }
            IsCharging = false;
        }

        public IEnumerator DoCharge()
        {
            // Set charging flag, reset parameters and indicators
            IsCharging = true;
            slowmotionStarted = false;
            ChargeIndicator.SetCurrent(0, true, true);
            float chargeTime = 0f;
            latestCharge = 0f;
            int previousChargeState = -1;

            // Wait until the charge time passes the deadzone
            while (chargeTime < ChargeDeadzone)
            {
                yield return new WaitForEndOfFrame();
                chargeTime += Time.unscaledDeltaTime;
            }

            // Start attack charge sound
            if (sounds.attackChargeSound == null)
                Debug.LogError("Attack charge sound is missing");
            else
                sounds.attackChargeSound.Play(); //------------------------------ charge sound

            // Activate the charge indicator
            if (ChargeIndicator != null && ChargeIndicator.Visualizer != null)
                ChargeIndicator.Visualizer.SetActive(true);

            // Start main charging loop
            while (IsCharging)
            {
                yield return new WaitForEndOfFrame();

                if (SlowmotionAffectsCharge)
                    chargeTime += Time.deltaTime;
                else
                    chargeTime += Time.unscaledDeltaTime;

                // If the player is standing inside charging prohibitors and cannot fully charge, reset the energy back to the end of the charging deadzone
                if (ChargingAllowed())
                    latestCharge = Mathf.Clamp01(chargeTime / ChargeTime);
                else
                {
                    latestCharge = Mathf.Clamp(chargeTime, 0f, ChargeDeadzone);
                    chargeTime = latestCharge;
                }

                // If the player does not have the required energy and cannot fully charge, clamp the charge to the limit.
                if (!energyCost.CanFullCharge)
                {
                    latestCharge = Mathf.Clamp(latestCharge, 0f, energyCost.EnergyRequirementTrigger);
                }

                // Evaluate the current charge state
                int currentChargeState = GetChargeIndex(latestCharge);

                // Compare the current charge state with the previous charge state. If it's different, change the indicator and play the corresponding tick sound
                if (currentChargeState != previousChargeState)
                {
                    ChargeIndicator.SetCurrent(currentChargeState + 1);
                    previousChargeState = currentChargeState;

                    //------------------------------------------------------------- Charge ticks
                    if (currentChargeState == 0) sounds.attackChargeTick1Sound.Play();
                    else if (currentChargeState == 1) sounds.attackChargeTick2Sound.Play();
                    else if (currentChargeState == 2) sounds.attackChargeTick3Sound.Play();
                    else if (currentChargeState == 3) sounds.attackChargeTick4Sound.Play();

                }

                // If slowmotion hasn't been started, check if the charge has passed the slowmotion trigger. If so, start slowmotion. If not, stop slowmotion.
                if (!slowmotionStarted)
                {
                    if (latestCharge > slowmotionTrigger)
                    {
                        slowmotionStarted = true;
                        TimeManager.Instance.DoSlowmotion(slowmotionFactor);
                    }
                }
                else
                {
                    if (latestCharge < slowmotionTrigger)
                    {
                        slowmotionStarted = false;
                        TimeManager.Instance.StopSlowmotion();
                    }
                }
            }

            // Once the loop has been exited, stop the slowmotion
            TimeManager.Instance.StopSlowmotion();

            // Deactivate the charge indicator visual
            if (ChargeIndicator != null && ChargeIndicator.Visualizer != null)
                ChargeIndicator.Visualizer.SetActive(false);

            // If the charge surpasses that of the energy requirement trigger, subtract energy from the character's energy pool
            if (latestCharge > energyCost.EnergyRequirementTrigger)
            {
                Debug.Log("Detract energy");
                energyCost.energyAbsorption.Energy -= energyCost.fullChargeCost;
            }

            // Stop the attack charge sound
            sounds.attackChargeSound?.Stop();
        }

        public void ApplyChargeZoneColors()
        {
            GameObject chargeObject = ChargeIndicator.Visualizer;
            if (chargeObject == null)
            {
                Debug.LogError("Charge object is not assigned");
                return;
            }
            Debug.Log("Apply charge zone colors");

            for (int i = 0; i < ChargeZones.colorKeys.Length; i++)
            {
                Color currentColor = ChargeZones.colorKeys[i].color;
                //Debug.Log("Color key " + i + ": " + currentColor);
                Transform child = chargeObject.transform.GetChild(i);
                if (child == null)
                {
                    Debug.LogError("Charge zone has no child at index " + i, chargeObject);
                }
                Graphic graphic = child.GetComponent<Graphic>();
                if (graphic == null)
                {
                    Debug.LogError("Transform child " + i + " of " + chargeObject + " has no Graphic component to set the color of", child);
                }
                //Debug.Log("Graphic " + graphic.name + "has been assigned color " + currentColor);
                graphic.color = currentColor;
            }
        }

        public void AttemptAttackCharge()
        {
            if (IsCharging)
            {
                Debug.Log("Already charging", this);
                return;
            }

            if (Character.stamina.IsEmpty(true))
            {
                return;
            }
            // TODO: Get stamina use from attack?
            Character.stamina.Use(1);

            StartCoroutine(DoCharge());
            Character.stamina.allowRecovery = false;
        }

        public void StartAttack(int attackIndex)
        {
            if (Character.Animator != null)
            {
                Character.Animator.SetInteger("AttackIndex", attackIndex);
                Character.Animator.SetTrigger("Attack");
            }
            Attack attack = GetAttack(attackIndex);
            SetMovementRestrictionsActive(attack, true);
            OnAttackStarted.Invoke();
            attack.damageObject.gameObject.SetActive(true);
        }

        public void SetMovementRestrictionsActive(Attack attack, bool active)
        {
            if (Character.Controller.movement == null) return;

            if (attack.restrictions.HasFlag(Restrictions.Move))
            {
                Character.Controller.movement.Stop();
                Character.Controller.movement.BlockMovementInput = active; // TODO: Misschien beter om het movement component uit te schakelen
            }
            if (attack.restrictions.HasFlag(Restrictions.Rotate))
                Character.Controller.movement.LockFacingDirection = active;
            if (attack.restrictions.HasFlag(Restrictions.StaminaRecovery))
                Character.stamina.allowRecovery = !active;
        }

        public void ResetMovementRestrictions()
        {
            if (Character.Controller.movement == null) return;
            Character.Controller.movement.BlockMovementInput = false;
            Character.Controller.movement.LockFacingDirection = false;
            Character.stamina.allowRecovery = true;
        }

        // Would be nice if this was available as a visual scripting block
        // Address this from the Animator, when leaving the attacking state
        public void EndAttack()
        {
            ResetMovementRestrictions();

            // Immediately update walking direction at end of attack 
            if (Character.Controller.movement != null)
                Character.Controller.movement.ForceReadMoveInput();
        }
    }
}