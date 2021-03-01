using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Combat;

namespace RanchyRats.Gyrus
{
    public class PlayerController : CharacterController
    {
        #region Julia Added

        [System.Serializable]
        public struct Sounds
        {
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




        //Later to be replaced by FMOD elements For now it's hard coded :^) You can find where I added something using the following indicator VVV
        //---------------------------------------------------------     (What it's about)
        //public

        #endregion
        //public UnityEvent OnHit;

        Controls controls;
        Controls Controls
        {
            get
            {
                if (controls == null)
                {
                    controls = new Controls();
                }
                return controls;
            }
        }

        Animator animator;
        Animator Animator
        {
            get
            {
                if (animator == null)
                {
                    animator = GetComponent<Animator>();
                    if (animator == null)
                        Debug.LogError("Animator is null", this);
                }
                return animator;
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            Controls.Game.Enable();
            SubscribeControls();

            movement.state = Movement.State.Idle;

            //Debug.Log("Set current health and stamina of " + name + " to max", this);

            LockCursor(true);
            attacking.state = Attacking.State.Ready;
            attacking.ClearChargingProhibitors();
        }

        private void OnDisable()
        {
            //Debug.Log("Player disabled");
            Controls.Game.Disable();
            UnsubControls();
            InterruptAttackCharge();

            LockCursor(false);
        }

        private void LockCursor(bool locked)
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                Debug.Log("Editor application is not playing. Unlocking cursor.");
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                return;
            }
#endif
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }

        bool controlsSubscribed = false;
        private void SubscribeControls()
        {
            if (controlsSubscribed)
                return;
            Controls.Game.Movement.performed += _ => movement.SetMoveInput(_.ReadValue<Vector2>());
            Controls.Game.Movement.canceled += _ => movement.Stop();

            Controls.Game.Dodge.performed += _ => movement.AttemptDodge();

            Controls.Game.Attack.performed += _ => AttemptAttackCharge();
            Controls.Game.Attack.canceled += _ => attacking.CompleteCharge();

            Controls.Game.LockOn.performed += _ => targeting.LockOn(transform.position);

            Controls.Game.Run.started += _ => movement.StartRunning();
            Controls.Game.Run.canceled += _ => movement.StopRunning();

            controlsSubscribed = true;
        }

        private void UnsubControls()
        {
            if (!controlsSubscribed)
                return;
            Controls.Game.Movement.performed -= _ => movement.SetMoveInput(_.ReadValue<Vector2>());
            Controls.Game.Movement.canceled -= _ => movement.Stop();

            Controls.Game.Dodge.performed -= _ => movement.AttemptDodge();

            Controls.Game.Attack.performed -= _ => AttemptAttackCharge();
            Controls.Game.Attack.canceled -= _ => attacking.CompleteCharge();

            Controls.Game.LockOn.performed -= _ => targeting.LockOn(transform.position);

            Controls.Game.Run.started -= _ => movement.StartRunning();
            Controls.Game.Run.canceled -= _ => movement.StopRunning();

            controlsSubscribed = false;
        }

        #region Movement
        public Movement movement;

        public void UpdateMoveInput()
        {
            Vector2 readMovement = Controls.Game.Movement.ReadValue<Vector2>();
            if (readMovement != Vector2.zero)
                movement.SetMoveInput(readMovement);
            else
                movement.Stop();
        }
        #endregion

        // Maak component
        #region Attacking
        [Serializable]
        public class Attacking
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
        }
        public Attacking attacking;


        public void InterruptAttackCharge()
        {
            StopCoroutine(attacking.DoCharge());
            if (attacking.slowmotionInitiated) TimeManager.Instance.StopSlowmotion();
            attacking.InterruptCharge();
        }


        public void AttemptAttackCharge()
        {
            Debug.Log("Attempt attack charge while in state " + attacking.state.ToString());
            switch (attacking.state)
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

            if (Character.Stamina.IsEmpty(true))
            {
                return;
            }
            Character.Stamina.Use(1);

            StartCoroutine(attacking.DoCharge());
            Character.Stamina.allowRecovery = false;
        }

        public void AddNoChargeZone(GameObject zone)
        {
            attacking.AddChargingProhibitor(zone);
        }

        public void RemoveNoChargeZone(GameObject zone)
        {
            attacking.RemoveChargingProhibitor(zone);
        }
        public bool AllowMovementWhileAttacking = false;
        public void OnAttack()
        {
            Animator.SetFloat("AttackCharge", attacking.latestCharge);
            Animator.SetTrigger("Attack");
            attacking.WeaponAnimator.SetFloat("AttackCharge", attacking.latestCharge);
            attacking.WeaponAnimator.SetTrigger("Attack");

            if (!AllowMovementWhileAttacking)
            {
                movement.Stop();
                movement.AcceptMovementInput = false;
                movement.enabled = false;
            }
            attacking.state = Attacking.State.Attacking;
            Character.Stamina.allowRecovery = false;
        }

        private void EndAttack()
        {
            //Debug.Log("On attack end from instance. Accept movement input again.", this.gameObject);
            attacking.state = Attacking.State.Ready;
            movement.state = Movement.State.Idle;
            movement.AcceptMovementInput = true;
            Character.Stamina.allowRecovery = true;
            // Immediately update walking direction at end of attack 
            UpdateMoveInput();
        }

        #endregion

        // Maak component
        #region Targeting
        [Serializable]
        public class Targeting
        {
            public GameObject DebugTarget;
            public GameObject GetTarget(Vector3 position)
            {
                if (DebugTarget != null)
                    return DebugTarget;
                Collider[] colliders = Physics.OverlapSphere(position, Radius, Targetable);
                if (colliders.Length > 0)
                    return colliders[0].gameObject;
                return null;
            }
            public float Radius = 3f;
            public Color RadiusColor = Color.white;
            public LayerMask Targetable;

            internal void LockOn(Vector3 position)
            {
                Debug.Log("Lock on");
            }
        }
        public Targeting targeting;

        public void LockOn(Vector3 position)
        {

        }
        #endregion

        public void DoGamepadRumble(float duration = .25f)
        {
            GamePadFunctions.Instance.DoGamepadRumble(duration);
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            Handles.color = targeting.RadiusColor;
            Handles.DrawWireDisc(transform.position, transform.up, targeting.Radius);
        }
#endif
    }
}