using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using BobJeltes.Attributes;
using System.Linq;
#if UNITY_EDITOR
#endif
using Combat;

namespace RanchyRats.Gyrus
{
    public class Attacking : CharacterComponent
    {
        [Serializable]
        public struct Sounds
        {
            public FMODUnity.StudioEventEmitter attackChargeSound;
            public FMODUnity.StudioEventEmitter attackChargeTick1Sound;
            public FMODUnity.StudioEventEmitter attackChargeTick2Sound;
            public FMODUnity.StudioEventEmitter attackChargeTick3Sound;
            public FMODUnity.StudioEventEmitter attackChargeTick4Sound;
        }
        public Sounds sounds;

        [Header("Charging")]
        public Slider ChargeSlider;
        //public Gradient ChargeZones;
        public Statistic ChargeIndicator;
        [Tooltip("Time it takes for the slider to fill up")]
        [Min(0)]
        public float ChargeTimeMax = 2f;
        [Tooltip("Time below which a charge will not be initiated")]
        [Min(0)]
        public float ChargeDeadzone = .1f;
        [Serializable]
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

        [Tooltip("If true, the charge will slow down, along with everything else. If false, the charge continues as quickly in, as it does out of slowmotion.")]
        public bool SlowmotionAffectsCharge = false;

        [Range(0f, 1f)]
        public float slowmotionTrigger = .5f;
        [Range(0f, 1f)]
        public float slowmotionFactor = .25f;

        internal bool slowmotionStarted = false;

        public bool AllowCharging = true;
        public bool IsCharging = false;
        public GameObject ChargeBlockedIndicator;
        // Number between 0 and 1
        private float latestCharge = 0f;
        // Raw time of latest charge
        private float latestChargeTime = 0f;
        private int previousState = -1;

        [Header("Attacks")]
        //[Tooltip("WARNING: NOT YET IMPLEMENTED. If true, uses the SingleWeaponObject. Otherwise, uses the attacks from the list.")]
        //public bool SingleWeaponObject = true;
        //public Animator WeaponAnimator;
        public Attack[] attacks = new Attack[0];
        [System.Serializable]
        public struct Attack
        {
            public int staminaCost;
            public Restrictions restrictions;
            public Damager damageObject;
            [Range(0f, 1f)]
            public float ChargeRequirement;
        }
        public Attack GetAttack(int index) => (index > attacks.Length - 1) ? attacks[attacks.Length - 1] : attacks[index];
        /// <summary>
        /// Gets the strongest attack that can be used with this amount of charge
        /// </summary>
        /// <param name="charge">A number between 0 and 1, indicating the progress of the charge</param>
        /// <returns>The strongest attack that can be used</returns>
        public Attack GetAttack(float charge) => attacks.Last(x => x.ChargeRequirement < charge);

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

        //int GetChargeIndex(float time)
        //{
        //    for (int i = 0; i < ChargeZones.colorKeys.Length; i++)
        //    {
        //        if (ChargeZones.colorKeys[i].time >= time)
        //        {
        //            //Debug.Log("Time: " + time + " index " + i);
        //            return i;
        //        }
        //    }
        //    Debug.Log("No index at time " + time + " found. Returning max: " + (ChargeZones.colorKeys.Length - 1));
        //    return ChargeZones.colorKeys.Length - 1;
        //}

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

        internal bool ChargingAllowed => chargeBlockers.Count == 0;

        public void StartCharge()
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

            // Set charging flag, reset parameters and indicators
            IsCharging = true;
            slowmotionStarted = false;
            ChargeIndicator.SetCurrent(0, true, true);
            latestCharge = 0f;
            previousState = -1;

            latestChargeTime = 0f;
            Character.stamina.allowRecovery = false;

            StartCoroutine(DoCharge());
        }

        public void EndCharge(bool complete)
        {
            if (!IsCharging)
            {
                Debug.Log("Not charging.");
                return;
            }

            // Once the loop has been exited, stop the slowmotion
            if (slowmotionStarted) TimeManager.Instance.StopSlowmotion();

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
            IsCharging = false;
            if (complete)
                StartAttack(latestCharge);
            else
                StopCoroutine(DoCharge());
        }

        public IEnumerator DoCharge()
        {
            // Wait until the charge time passes the deadzone
            while (latestChargeTime < ChargeDeadzone)
            {
                yield return new WaitForEndOfFrame();
                latestChargeTime += Time.unscaledDeltaTime;
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
                    latestChargeTime += Time.deltaTime;
                else
                    latestChargeTime += Time.unscaledDeltaTime;

                // If the player is standing inside charging prohibitors and cannot fully charge, reset the energy back to the end of the charging deadzone
                if (ChargingAllowed)
                    latestCharge = Mathf.Clamp01(latestChargeTime / ChargeTimeMax);
                else
                {
                    latestCharge = Mathf.Clamp(latestChargeTime, 0f, ChargeDeadzone);
                    latestChargeTime = latestCharge;
                }

                // If the player does not have the required energy and cannot fully charge, clamp the charge to the limit.
                if (!energyCost.CanFullCharge)
                {
                    latestCharge = Mathf.Clamp(latestCharge, 0f, energyCost.EnergyRequirementTrigger);
                }

                // TODO: Evaluate the current charge state. Do this using the attack charge requirements, and getting the attack's index
                //int currentChargeState = GetChargeIndex(latestCharge);
                //Attack attack = attacks.Last(x => x.ChargeRequirement < latestCharge);
                //attacks.
                //attacks.LastOrDefault();
                //attacks.FindIndex();
                List<Attack> newAttacks = new List<Attack>();
                newAttacks.FindLastIndex(0, x => x.ChargeRequirement < latestCharge);
                newAttacks.FindIndex(x => x.ChargeRequirement < latestCharge);


                // Compare the current charge state with the previous charge state. If it's different, change the indicator and play the corresponding tick sound
                //if (currentChargeState != previousState)
                //{
                //    ChargeIndicator.SetCurrent(currentChargeState + 1);
                //    previousState = currentChargeState;

                //    //------------------------------------------------------------- Charge ticks
                //    if (currentChargeState == 0) sounds.attackChargeTick1Sound.Play();
                //    else if (currentChargeState == 1) sounds.attackChargeTick2Sound.Play();
                //    else if (currentChargeState == 2) sounds.attackChargeTick3Sound.Play();
                //    else if (currentChargeState == 3) sounds.attackChargeTick4Sound.Play();
                //}

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

                // IDEA: To prevent players from holding their charge indefinitely, make the charge cost stamina for every realtime second held while it's fully charged
                if (latestCharge == 1f) Character.stamina.Use(1);
            }
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

            //for (int i = 0; i < ChargeZones.colorKeys.Length; i++)
            //{
            //    Color currentColor = ChargeZones.colorKeys[i].color;
            //    //Debug.Log("Color key " + i + ": " + currentColor);
            //    Transform child = chargeObject.transform.GetChild(i);
            //    if (child == null)
            //    {
            //        Debug.LogError("Charge zone has no child at index " + i, chargeObject);
            //    }
            //    Graphic graphic = child.GetComponent<Graphic>();
            //    if (graphic == null)
            //    {
            //        Debug.LogError("Transform child " + i + " of " + chargeObject + " has no Graphic component to set the color of", child);
            //    }
            //    //Debug.Log("Graphic " + graphic.name + "has been assigned color " + currentColor);
            //    graphic.color = currentColor;
            //}
        }

        public void StartAttack(float charge) => StartAttack(GetAttack(charge));

        public void StartAttack(int attackIndex) => StartAttack(GetAttack(attackIndex));

        public void StartAttack(Attack attack)
        {
            if (Character.Animator != null)
                Character.Animator.SetTrigger("Attack");
            SetMovementRestrictionsActive(attack, true);
            Character.stamina.Use(attack.staminaCost);
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