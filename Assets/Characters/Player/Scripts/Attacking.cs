using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#if UNITY_EDITOR
#endif
using Combat;

namespace RanchyRats.Gyrus
{
    public class Attacking : CharacterComponent
    {
        public FMODUnity.StudioEventEmitter attackChargeSound;
        public FMODUnity.StudioEventEmitter attackChargeTik1Sound;
        public FMODUnity.StudioEventEmitter attackChargeTik2Sound;
        public FMODUnity.StudioEventEmitter attackChargeTik3Sound;
        public FMODUnity.StudioEventEmitter attackChargeTik4Sound;

        public Slider ChargeSlider;
        //public GameObject ChargeIndicators;
        //public enum EChargeType
        //{
        //    States,
        //    Slider,
        //    Both
        //}
        //public EChargeType ChargeType;
        public Animator WeaponAnimator;
        public Gradient ChargeZones;
        public Statistic ChargeIndicator;
        [Tooltip("Time it takes for the slider to fill up")]
        public float ChargeTime = 2f;
        [Tooltip("Time below which a charge will not be initiated")]
        public float ChargeTimeDeadzone = .1f;
        public bool ChargeEffectedBySlowdown = false;

        [Range(0f, 1f)]
        public float slowmotionTrigger = .5f;
        [Range(0f, 1f)]
        public float slowmotionFactor = .25f;

        internal bool allowCharging = true;
        public GameObject ChargeDisabledIndicator;
        internal float latestCharge;
        public UnityEvent attackLaunched;
        public UnityEvent attackEnd;

        public enum State
        {
            Ready,
            Charging,
            Attacking,
            OnCooldown,
            Disabled
        }
        public State state;

        private void OnDisable()
        {
            InterruptCharge();
        }

        internal void Launch(int chargeIndex)
        {
            WeaponAnimator.SetTrigger("Attack");
            WeaponAnimator.SetInteger("AttackIndex", chargeIndex);

            attackLaunched.Invoke();
        }

        internal void Launch(float chargeTime)
        {
            if (chargeTime == 0f)
            {
                Debug.Log("Launch uncharged attack!");
            }
            else
            {
                //string colorString = GetChargeZone(chargeTime).ToString();
                //string chargeAmount = GetChargeZone(chargeTime, EvaluatedColorChannel).ToString();
                //Debug.Log("Launch charged attack. Charge amount: <color=" + colorString + ">" + chargeAmount + "</color>");
                Debug.Log("Launch charged attack. Charge amount: " + chargeTime);
            }
            latestCharge = chargeTime;
            attackLaunched.Invoke();
        }

        int GetChargeZoneIndex(float time)
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

        public void InterruptCharge()
        {
            StopCoroutine(DoCharge());
            if (slowmotionInitiated) TimeManager.Instance.StopSlowmotion();
            if (state != State.Charging)
            {
                Debug.Log("No charge to interrupt");
                return;
            }
            ChargeIndicator.SetCurrent(0);
            state = State.Ready;
        }

        public void CompleteCharge()
        {
            if (state != State.Charging)
            {
                Debug.Log("No charge to complete");
                return;
            }
            state = State.Attacking;
        }

        [Tooltip("The number of triggers that the player is inside of, prohibiting its charge")]
        internal List<GameObject> chargeProhibitors = new List<GameObject>();

        internal void AddChargingProhibitor(GameObject prohibitor)
        {
            chargeProhibitors.Add(prohibitor);
            ChargeDisabledIndicator.SetActive(chargeProhibitors.Count != 0);
        }

        internal void RemoveChargingProhibitor(GameObject prohibitor)
        {
            chargeProhibitors.Remove(prohibitor);
            ChargeDisabledIndicator.SetActive(chargeProhibitors.Count != 0);
        }

        internal void ClearChargingProhibitors()
        {
            chargeProhibitors.Clear();
            ChargeDisabledIndicator.SetActive(false);
        }

        internal bool ChargingAllowed()
        {
            return chargeProhibitors.Count == 0;
        }

        public EnergyAbsorption energyAbsorption;
        public int fullChargeCost = 20;
        [Range(0f, 1f)]
        public float EnergyChargeLimit = .75f;

        internal bool CanFullCharge()
        {
            if (energyAbsorption == null) return true;
            return energyAbsorption.Energy >= fullChargeCost;
        }

        internal bool slowmotionInitiated = false;

        public IEnumerator DoCharge()
        {
            state = State.Charging;

            ChargeIndicator.SetCurrent(0, true, true);
            float chargeTime = 0f;
            float chargeTimeClamped = 0f;
            //Debug.Log("Start charge");
            while (state == State.Charging && chargeTime < ChargeTimeDeadzone)
            {
                yield return new WaitForEndOfFrame();
                chargeTime += Time.unscaledDeltaTime;
            }

            if (attackChargeSound == null)
                Debug.LogError("Attack charge sound is missing");
            else
                attackChargeSound.Play(); //------------------------------ charge sound

            //Debug.Log("Charge deadzone passed");
            ChargeIndicator.Visualizer.SetActive(true);
            slowmotionInitiated = false;
            int previousChargeState = -1;
            while (state == State.Charging)
            {
                yield return new WaitForEndOfFrame();
                if (ChargeEffectedBySlowdown)
                {
                    chargeTime += Time.deltaTime;
                }
                else
                {
                    chargeTime += Time.unscaledDeltaTime;
                }

                if (ChargingAllowed())
                {
                    chargeTimeClamped = Mathf.Clamp01(chargeTime / ChargeTime);
                }
                else
                {
                    chargeTimeClamped = Mathf.Clamp(chargeTime, 0f, ChargeTimeDeadzone);
                    chargeTime = chargeTimeClamped;
                }

                if (!CanFullCharge())
                {
                    chargeTimeClamped = Mathf.Clamp(chargeTimeClamped, 0f, EnergyChargeLimit);
                }

                //Debug.Log("Chargetime clamped: " + chargeTimeClamped);

                int currentChargeState = GetChargeZoneIndex(chargeTimeClamped);

                if (currentChargeState != previousChargeState)
                {
                    ChargeIndicator.SetCurrent(currentChargeState + 1);
                    previousChargeState = currentChargeState;

                    //------------------------------------------------------------- Charge tiks
                    if (currentChargeState == 0) attackChargeTik1Sound.Play();
                    else if (currentChargeState == 1) attackChargeTik2Sound.Play();
                    else if (currentChargeState == 2) attackChargeTik3Sound.Play();
                    else if (currentChargeState == 3) attackChargeTik4Sound.Play();

                }
                if (!slowmotionInitiated)
                {
                    if (chargeTimeClamped > slowmotionTrigger)
                    {
                        slowmotionInitiated = true;
                        TimeManager.Instance.DoSlowmotion(slowmotionFactor);
                    }
                }
                else
                {
                    if (chargeTimeClamped < slowmotionTrigger)
                    {
                        slowmotionInitiated = false;
                        TimeManager.Instance.StopSlowmotion();
                    }
                }
            }
            TimeManager.Instance.StopSlowmotion();
            ChargeIndicator.Visualizer.SetActive(false);

            //Launch(chargeTimeClamped);
            if (chargeTimeClamped > EnergyChargeLimit)
            {
                Debug.Log("Detract energy");
                energyAbsorption.Energy -= fullChargeCost;
            }

            attackChargeSound.Stop();
            if (state == State.Attacking)
                Launch(GetChargeZoneIndex(chargeTimeClamped));
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
            Debug.Log("Attempt attack charge while in state " + state.ToString());
            switch (state)
            {
                // Attack charge allowed when
                case Attacking.State.Ready:
                case Attacking.State.OnCooldown:
                    break;
                // Attack charge not allowed when
                case Attacking.State.Disabled:
                case Attacking.State.Charging:
                case Attacking.State.Attacking:
                    return;
            }

            if (Character.stamina.IsEmpty(true))
            {
                return;
            }
            Character.stamina.Use(1);

            StartCoroutine(DoCharge());
            Character.stamina.allowRecovery = false;
        }

        public void AddNoChargeZone(GameObject zone)
        {
            AddChargingProhibitor(zone);
        }

        public void RemoveNoChargeZone(GameObject zone)
        {
            RemoveChargingProhibitor(zone);
        }
        public bool AllowMovementWhileAttacking = false;
        public void OnAttack()
        {
            if (Character.Animator != null)
            {
                Character.Animator.SetFloat("AttackCharge", latestCharge);
                Character.Animator.SetTrigger("Attack");
            }
            WeaponAnimator.SetFloat("AttackCharge", latestCharge);
            WeaponAnimator.SetTrigger("Attack");
            // TODO: Move this to a callback in CharacterController
            if (!AllowMovementWhileAttacking)
            {
                if (Character.movement != null)
                {
                    Character.movement.Stop();
                    Character.movement.AcceptMovementInput = false;
                    Character.movement.enabled = false;
                }
            }
            state = Attacking.State.Attacking;
            Character.stamina.allowRecovery = false;
        }

        private void EndAttack()
        {
            //Debug.Log("On attack end from instance. Accept movement input again.", this.gameObject);
            state = Attacking.State.Ready;
            if (Character.movement != null)
            {
                Character.movement.state = Movement.State.Idle;
                Character.movement.AcceptMovementInput = true;
            }
            Character.stamina.allowRecovery = true;

            // Immediately update walking direction at end of attack 
            if (Character.movement != null)
            {
                Character.movement.ForceReadMoveInput();
            }
        }
    }
}