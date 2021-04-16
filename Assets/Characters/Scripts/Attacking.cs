using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using BobJeltes.Events;
using Gyrus.Combat;

namespace RanchyRats.Gyrus
{
    public class Attacking : CharacterComponent
    {
        [Header("Charging")]
        public ChargeEvents chargeEvents = new ChargeEvents();
        [Serializable]
        public struct ChargeEvents
        {
            public UnityEvent OnChargeStarted;
            public UnityEvent OnChargeStopped;
            [Header("Passes the percentual charge (between 0 and 1")]
            public UnityEventFloat OnChargeChanged;
        }
        public Slider ChargeSlider;
        //public Gradient ChargeZones;
        public Stat ChargeIndicator;
        [Tooltip("Time it takes for the slider to fill up")]
        [Min(0)]
        public float ChargeTimeMax = 2f;
        [Tooltip("Time below which a charge will not be initiated")]
        [Min(0)]
        public float ChargeTimeDeadzone = .1f;
        [Serializable]
        public struct EnergyCost
        {
            public EnergyAbsorption energyAbsorption;
            [Min(0)]
            public int fullChargeCost;
            [Min(0)] [Tooltip("The time at which the player's charge is halted, if the character does not posess the required energy.")]
            public float ChargeLimitTime;
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
        [Serializable]
        public struct Slowmotion
        {
            [Tooltip("If true, the charge will slow down, along with everything else. If false, the charge continues as quickly in, as it does out of slowmotion.")]
            public bool AffectsCharge;

            [Range(0f, 1f)]
            public float Trigger;
            [Range(0f, 1f)]
            public float Factor;

            internal bool active;

            public Slowmotion(bool affectsCharge)
            {
                AffectsCharge = affectsCharge;
                Trigger = .5f;
                Factor = .25f;
                active = false;
            }
        }
        public Slowmotion slowmotion = new Slowmotion(false);

        public bool AllowCharging = true;
        public bool IsCharging = false;
        [Serializable]
        public struct FullChargeStaminaDrain
        {
            public bool enabled;
            [Tooltip("When stamina is depleted by holding a full charge, the charge is interrupted when this box is ticked, causing the attack to be cancelled.")]
            public bool DepletionInterruptsCharge;
            [Min(0f)]
            public float DrainInterval;
            public int DrainAmount;
            public float TimeBeforeDrain;

            /// <summary>
            /// Updates the time before the next drain should take place
            /// </summary>
            /// <returns>Whether the time before the next drain has run out, and stamina should be drained</returns>
            public bool Update()
            {
                // To prevent players from holding their charge indefinitely, make the charge cost stamina for every --realtime-- second held while it's fully charged
                if (enabled)
                {
                    TimeBeforeDrain -= Time.unscaledDeltaTime;
                    if (TimeBeforeDrain <= 0f)
                    {
                        TimeBeforeDrain = DrainInterval;
                        return true;
                    }
                }
                return false;
            }
        }
        public FullChargeStaminaDrain fullChargeStaminaDrain = new FullChargeStaminaDrain { DrainInterval = 1f, TimeBeforeDrain = 0, DrainAmount = 1 };
        public bool IsFullyCharged => LatestCharge >= 1f;
        public GameObject ChargeBlockedIndicator;
        internal bool ChargingBlocked => chargeBlockers.Count > 0;
        private float LatestCharge => Mathf.Clamp(latestChargeTime, 0f, ChargeTimeDeadzone);
        // Raw time of latest charge
        private float latestChargeTime = 0f;
        private int previousIndex = -1;

        [Header("Attacks")]
        public List<Attack> attacks = new List<Attack>();
        [System.Serializable]
        public struct Attack
        {
            public int staminaCost;
            public Restrictions restrictions;
            public global::Gyrus.Combat.Attack attackObject;
            [Range(0f, 1f)]
            public float ChargeRequirement;
        }
        public Attack GetAttack(int index) => (index > attacks.Count - 1) ? attacks[attacks.Count - 1] : attacks[index];

        /// <summary>
        /// Gets the strongest attack that can be used with this amount of charge
        /// </summary>
        /// <param name="charge">A number between 0 and 1, indicating the progress of the charge</param>
        /// <returns>The strongest attack that can be used</returns>
        public Attack GetAttack(float charge)
        {
            attacks.OrderBy(x => x.ChargeRequirement);
            return attacks.Last(x => x.ChargeRequirement <= charge);
        }

        [Flags] public enum Restrictions
        {
            None = 0,
            Move = 1,
            Rotate = 2,
            StaminaRecovery = 4
        }
        [Serializable]
        public struct AttackEvents
        {
            public UnityEvent OnAttackAnnouncement;
            public UnityEvent OnAttackStarted;
            public UnityEvent OnAttackEnded;
        }
        public AttackEvents attackEvents;

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
            for (int i = 0; i < attacks.Count; i++)
            {
                if (attacks[i].attackObject == null) continue;
                attacks[i].attackObject.gameObject.SetActive(false);
                attacks[i].attackObject.events.OnDeactivation.AddListener(EndAttack);
            }
        }

        private void OnDisable()
        {
            EndCharge(false);
            for (int i = 0; i < attacks.Count; i++)
            {
                if (attacks[i].attackObject == null) continue;
                attacks[i].attackObject.events.OnDeactivation.RemoveListener(EndAttack);
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
            slowmotion.active = false;
            ChargeIndicator.Value = 0;
            previousIndex = -1;

            latestChargeTime = 0f;
            Character.stamina.allowRecharge = false;

            StartCoroutine(DoCharge());

            chargeEvents.OnChargeStarted.Invoke();
        }

        public void EndCharge(bool complete)
        {
            if (!IsCharging)
            {
                Debug.Log("Not charging.");
                return;
            }

            // Once the loop has been exited, stop the slowmotion
            if (slowmotion.active) TimeManager.Instance.StopSlowmotion();

            // Deactivate the charge indicator visual
            if (ChargeIndicator != null && ChargeIndicator.Visualizer != null)
                ChargeIndicator.Visualizer.SetActive(false);

            // If the charge surpasses that of the energy requirement trigger, subtract energy from the character's energy pool
            if (LatestCharge > energyCost.ChargeLimitTime)
            {
                Debug.Log("Detract energy");
                energyCost.energyAbsorption.Energy -= energyCost.fullChargeCost;
            }
            chargeEvents.OnChargeStopped.Invoke();
            IsCharging = false;
            if (complete)
                StartAttack(LatestCharge);
            else
                StopCoroutine(DoCharge());
        }

        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        public IEnumerator DoCharge()
        {
            // Wait until the charge time passes the deadzone
            while (latestChargeTime < ChargeTimeDeadzone)
            {
                latestChargeTime += Time.unscaledDeltaTime;
                yield return waitForEndOfFrame;
            }

            // Start main charging loop
            while (IsCharging)
            {
                // Add charge time
                latestChargeTime += slowmotion.AffectsCharge ? Time.deltaTime : Time.unscaledDeltaTime;

                // If the player is standing inside charge blockers and cannot fully charge, reset the energy back to the end of the charging deadzone
                if (ChargingBlocked)
                    latestChargeTime = 0f;
                else
                    latestChargeTime = Mathf.Min(latestChargeTime, ChargeTimeMax);

                // If the player does not have the required energy and cannot fully charge, clamp the charge to the limit.
                if (!energyCost.CanFullCharge)
                    latestChargeTime = Mathf.Clamp(latestChargeTime, 0f, energyCost.ChargeLimitTime);

                // TODO: Evaluate the current charge state. Do this using the attack charge requirements, and getting the attack's index
                int currentIndex = attacks.FindIndex(x => x.Equals(GetAttack(LatestCharge)));

                // Compare the current charge state with the previous charge state. If it's different, change the indicator and play the corresponding tick sound
                if (currentIndex != previousIndex)
                {
                    ChargeIndicator.Value = currentIndex + 1;
                    previousIndex = currentIndex;
                    chargeEvents.OnChargeChanged.Invoke(LatestCharge);
                }

                // If slowmotion hasn't been started, check if the charge has passed the slowmotion trigger. If so, start slowmotion. If not, stop slowmotion.
                if (!slowmotion.active)
                {
                    if (LatestCharge > slowmotion.Trigger)
                    {
                        slowmotion.active = true;
                        TimeManager.Instance.DoSlowmotion(slowmotion.Factor);
                    }
                }
                else
                {
                    if (LatestCharge < slowmotion.Trigger)
                    {
                        slowmotion.active = false;
                        TimeManager.Instance.StopSlowmotion();
                    }
                }

                if (fullChargeStaminaDrain.Update())
                {
                    if (IsFullyCharged) Character.stamina.Use(fullChargeStaminaDrain.DrainAmount);
                    if (fullChargeStaminaDrain.DepletionInterruptsCharge && Character.stamina.IsEmpty(true))
                        EndCharge(false);
                }

                yield return waitForEndOfFrame;
            }
        }

        public void ApplyChargeZoneColors()
        {
            //GameObject chargeObject = ChargeIndicator.Visualizer;
            //if (chargeObject == null)
            //{
            //    Debug.LogError("Charge object is not assigned");
            //    return;
            //}
            //Debug.Log("Apply charge zone colors");

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
            attackEvents.OnAttackStarted.Invoke();
            attack.attackObject.gameObject.SetActive(true);
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
                Character.stamina.allowRecharge = !active;
        }

        public void ResetMovementRestrictions()
        {
            if (Character.Controller.movement == null) return;
            Character.Controller.movement.BlockMovementInput = false;
            Character.Controller.movement.LockFacingDirection = false;
            Character.stamina.allowRecharge = true;
        }

        // Would be nice if this was available as a visual scripting block
        // Address this from the Animator, when leaving the attacking state
        public void EndAttack()
        {
            ResetMovementRestrictions();

            // Immediately update walking direction at end of attack 
            if (Character.Controller.movement != null)
                Character.Controller.movement.ForceReadMoveInput();
            attackEvents.OnAttackEnded.Invoke();
        }
    }
}