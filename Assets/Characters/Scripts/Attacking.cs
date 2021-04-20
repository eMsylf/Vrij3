using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using BobJeltes.Events;
using Gyrus;

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
        public CharacterStatistic ChargeIndicator;
        [Tooltip("Time it takes for the slider to fill up")]
        [Min(0)]
        public float ChargeTimeMax = 2f;
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
        private bool IsCharging = false;
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
        private float LatestCharge
        {
            get
            {
                // TODO: Doe hier iets anders leuks waardoor shit niet breekt dankjewel
                return Mathf.InverseLerp(0f, ChargeTimeMax, latestChargeTime);
            }
        }

        // Raw time of latest charge
        private float latestChargeTime = 0f;
        private int previousIndex = -1;
        [Header("Attacks")]
        public Attack unchargedAttack;
        [Tooltip("Make sure that the first attack has no charge requirement")]
        public List<Attack> chargedAttacks = new List<Attack>();
        public Attack GetChargedAttack(int index) => (index > chargedAttacks.Count - 1) ? chargedAttacks[chargedAttacks.Count - 1] : chargedAttacks[index];

        /// <summary>
        /// Gets the strongest attack that can be used with this amount of charge
        /// </summary>
        /// <param name="chargeRequirement">A value between 0 and 1, indicating the progress of the charge required to start an attack</param>
        /// <returns>The strongest attack that can be used</returns>
        public Attack GetChargedAttack(float chargeRequirement)
        {
            chargedAttacks.OrderBy(x => x.ChargeRequirement);
            return chargedAttacks.Last(x => x.ChargeRequirement <= chargeRequirement);
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

            if (unchargedAttack != null)
            {
                unchargedAttack.gameObject.SetActive(false);
                unchargedAttack.events.OnDeactivation.AddListener(EndAttack);
            }

            for (int i = 0; i < chargedAttacks.Count; i++)
            {
                if (chargedAttacks[i]== null) continue;
                chargedAttacks[i].gameObject.SetActive(false);
                chargedAttacks[i].events.OnDeactivation.AddListener(EndAttack);
            }
        }

        private void OnDisable()
        {
            EndCharge(false);
            if (unchargedAttack != null)
            {
                unchargedAttack.events.OnDeactivation.RemoveListener(EndAttack);
            }
            for (int i = 0; i < chargedAttacks.Count; i++)
            {
                if (chargedAttacks[i] == null) continue;
                chargedAttacks[i].events.OnDeactivation.RemoveListener(EndAttack);
            }
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

        public void StartCharge()
        {
            Debug.Log("Start charge");
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
            if (ChargeIndicator == null)
                Debug.Log("Charge indicator not assigned", this);
            else
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
            if (ChargeIndicator != null && ChargeIndicator.TransformwiseVisualizer != null)
                ChargeIndicator.SetValueWithoutEvent(0f);

            // If the charge surpasses that of the energy requirement trigger, subtract energy from the character's energy pool
            if (LatestCharge > energyCost.ChargeLimitTime)
            {
                Debug.Log("Detract energy");
                if (energyCost.energyAbsorption == null)
                    Debug.Log("Energy absorbtion component not assigned", this);
                else
                    energyCost.energyAbsorption.Energy -= energyCost.fullChargeCost;
            }
            chargeEvents.OnChargeStopped.Invoke();
            IsCharging = false;
            if (complete)
                StartAttack(LatestCharge);
            StopCoroutine(DoCharge());
        }

        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        public IEnumerator DoCharge()
        {
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
                    latestChargeTime = Mathf.Clamp(latestChargeTime, 0f, energyCost.ChargeLimitTime); // TODO: Doe dit niet met charge time maar met charge progress (0-1) voor versimpeling

                // TODO: Evaluate the current charge state. Do this using the attack charge requirements, and getting the attack's index
                int currentIndex = chargedAttacks.FindIndex(x => x.Equals(GetChargedAttack(LatestCharge)));
                Debug.Log("Latest charge: " + LatestCharge);

                // Compare the current charge state with the previous charge state. If it's different, change the indicator and play the corresponding tick sound
                if (currentIndex != previousIndex)
                {
                    if (ChargeIndicator != null)
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

        public void StartUnchargedAttack()
        {
            StartAttack(unchargedAttack);
        }

        public void StartAttack(float charge) => StartAttack(GetChargedAttack(charge));

        public void StartAttack(int attackIndex) => StartAttack(GetChargedAttack(attackIndex));

        public void StartAttack(Attack attack)
        {
            Debug.Log("Start attack");
            if (attack == null)
            {
                Debug.Log("Attack was null");
                return;
            }

            if (Character.Animator != null)
                Character.Animator.SetBool("IsAttacking", true);
            SetMovementRestrictions(attack);
            Character.stamina.Use(attack.staminaCost);
            attackEvents.OnAttackStarted.Invoke();
            attack.gameObject.SetActive(true);
        }

        public void SetMovementRestrictions(Attack attack)
        {
            if (Character.Controller.movement == null) return;

            if (attack.restrictions.HasFlag(Restrictions.Move))
            {
                Character.Controller.movement.Stop();
                Character.Controller.movement.LockPosition = true; // TODO: Misschien beter om het movement component uit te schakelen
            }
            if (attack.restrictions.HasFlag(Restrictions.Rotate))
                Character.Controller.movement.LockFacingDirection = true;
            if (attack.restrictions.HasFlag(Restrictions.StaminaRecovery))
                Character.stamina.allowRecharge = !false;
        }

        public void ResetMovementRestrictions()
        {
            if (Character.Controller.movement == null) return;
            Character.Controller.movement.LockPosition = false;
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
            if (Character.Animator != null) Character.Animator.SetBool("IsAttacking", false);
        }
    }
}