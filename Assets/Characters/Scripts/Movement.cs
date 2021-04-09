﻿using BobJeltes.Extensions;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using Combat;
using System.Collections;
using UnityEngine.Events;

namespace RanchyRats.Gyrus
{
    [RequireComponent(typeof(Rigidbody))]
    public partial class Movement : CharacterComponent
    {
        public bool BlockMovementInput = false;
        [Tooltip("If the movement is being controlled by a NavMeshAgent component, the movement animation is still updated through this component but the movement is not applied to the rigidbody.")]
        public bool MovementByNavMeshAgent = false;
        internal Vector2 Input;
        internal Vector2 FacingDirection = new Vector2(0f, 1f);

        internal enum DefaultDodgeDirection { Backward, ToCamera }
        [SerializeField] internal DefaultDodgeDirection defaultDodgeDirection = DefaultDodgeDirection.Backward;
        internal Vector2 DodgeDirection;

        [System.Serializable]
        public struct Events
        {
            public UnityEvent OnDodgeStarted;
            public UnityEvent OnDodgeCompleted;
        }

        // TODO: Use charachter stamina instead of separate reference
        public Stat Stamina;

        public Direction Direction;

        //Hey Julia here, I'm just throwing extra code things in here for now and will add comments where I also added something.
        public ParticleSystem dust;

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        Rigidbody rigidbody;
        public Rigidbody Rigidbody
        {
            get
            {
                if (rigidbody == null)
                {
                    rigidbody = GetComponent<Rigidbody>();
                }
                return rigidbody;
            }
        }

        public enum State
        {
            Stopped = default,
            Walking,
            Running,
            Dodging
        }
        public State state = default;
        // TODO: Zou chill zijn als deze als een tabel weergegeven zouden kunnen worden
        public MovementStateSettings StoppedSettings = new MovementStateSettings(0, 0);
        public MovementStateSettings WalkingSettings = new MovementStateSettings(1f, .5f);
        public MovementStateSettings RunningSettings = new MovementStateSettings(1.5f, .25f);
        public float runningAnimationMultiplier = 1.5f;
        private float runStaminaDrainTimeRemaining;
        public MovementStateSettings DodgingSettings = new MovementStateSettings(2f, 0, true, .3f, State.Stopped, true, 1, 0);
        public Events dodgeEvents;

        public MovementStateSettings GetStateSettings(State state)
        {
            switch (state)
            {
                case State.Stopped:
                default:
                    return StoppedSettings;
                case State.Walking:
                    return WalkingSettings;
                case State.Running:
                    return RunningSettings;
                case State.Dodging:
                    return DodgingSettings;
            }
        }

        private void OnEnable()
        {
            state = State.Stopped;
        }

        private void FixedUpdate()
        {
            if (MovementByNavMeshAgent)
                return;
            Vector3 playerMovement = GetTopDownMovement(Input, state) * GetStateSettings(state).speed;
            // TODO: Deze camera-relatieve berekening moet uitgevoerd worden in de Player Controller. De movement hoort dat niet te doen, omdat de AI geen camera heeft.
            // Of, de camera kan worden doorgegeven vanuit... ergens
            Rigidbody.MovePosition(Rigidbody.position + playerMovement.ConvertToObjectRelative(Camera.main.transform, true, true) * Time.fixedDeltaTime);
        }

        private void Update()
        {
            switch (state)
            {
                case State.Stopped:
                case State.Dodging:
                    break;
                case State.Walking:
                    break;
                case State.Running:
                    // TODO: Manage footstep sounds here too
                    ManageRunningStaminaDrain();
                    break;
            }
        }

        #region It would be nice if I could combine these somehow
        internal Vector3 GetTopDownMovement(Vector2 input, State state)
        {
            Vector3 movement = Vector3.zero;
            switch (state)
            {
                case State.Walking:
                    movement = new Vector3(input.x, 0f, input.y);
                    break;
                case State.Running:
                    movement = new Vector3(input.x, 0f, input.y);
                    break;
                case State.Dodging:
                    movement = new Vector3(DodgeDirection.x, 0f, DodgeDirection.y);
                    break;
            }
            return movement;
        }
        #endregion

        // Wanneer een aanval eindigt moet de input van de speler gelezen worden zodat de speler gelijk doorgaat met bewegen als de stick in een richting gehouden wordt tijdens de aanval. Zonder dit blijft de speler stilstaan.
        public void ForceReadMoveInput()
        {
            if (Character.Controller.PlayerController == null)
                return;

            Vector2 input = (Character.Controller.PlayerController).Controls.Game.Movement.ReadValue<Vector2>();
            if (input != Vector2.zero)
            {
                SetMoveInput(input);
                bool run = (Character.Controller.PlayerController).Controls.Game.Run.phase == UnityEngine.InputSystem.InputActionPhase.Performed;
                if (run)
                    StartRunning();
            }
            else
                Stop();
        }

        public void SetMoveInput(Vector2 input)
        {
            if (BlockMovementInput)
            {
                //Debug.LogWarning("Tried to move while input was not accepted");
                return;
            }
            //Debug.Log("Input: " + input);

            switch (state)
            {
                case State.Dodging:
                    Debug.Log("Player can't move while " + state.ToString());
                    return;
                case State.Stopped:
                    state = State.Walking;
                    break;
                case State.Walking:
                    break;
            }
            Input = input;
            if (Character.Animator != null)
                Character.Animator.SetBool("IsWalking", true);
            UpdateFacingDirection(Input);
        }

        public void Stop()
        {
            //Debug.Log("Stop!");
            Input = Vector2.zero;
            //UpdateAnimatorDirection(Direction.UpdateLookDirection(MovementInput));
            state = State.Stopped;
            if (Character.Animator != null)
                Character.Animator.SetBool("IsWalking", false);
        }

        public void StartRunning()
        {
            if (RunningSettings.drainsStamina && Stamina != null && Stamina.IsEmpty(false))
                return;
            switch (state)
            {
                case State.Stopped:
                case State.Walking:
                case State.Running:
                    state = State.Running;
                    break;
                case State.Dodging:
                    return;
            }
            runStaminaDrainTimeRemaining = GetStateSettings(State.Running).staminaDrainInterval;
            Character.Animator.SetFloat("RunningMultiplier", runningAnimationMultiplier);
        }

        public void StopRunning()
        {
            switch (state)
            {
                case State.Stopped:
                default:
                case State.Walking:
                case State.Dodging:
                    break;
                case State.Running:
                    state = State.Walking;
                    break;
            }
            Character.Animator.SetFloat("RunningMultiplier", 1f);
        }

        private void ManageRunningStaminaDrain()
        {
            if (!RunningSettings.drainsStamina)
                return;
            // TODO: Implement stamina drain settings
            runStaminaDrainTimeRemaining -= Time.deltaTime;

            if (runStaminaDrainTimeRemaining <= 0f)
            {
                runStaminaDrainTimeRemaining = GetStateSettings(State.Running).staminaDrainInterval;
                Stamina.Use(1);

                if (Stamina.IsEmpty(true))
                {
                    StopRunning();
                }
            }
        }

        public void AttemptDodge()
        {
            switch (state)
            {
                // Dodging is allowed when
                case State.Stopped:
                case State.Walking:
                case State.Running:
                    break;
                // Dodging is not allowed when
                case State.Dodging:
                    return;
            }

            if (DodgingSettings.drainsStamina && Stamina != null && Stamina.IsEmpty(true))
            {
                //Debug.Log("Insufficient stamina to dodge");
                //-----------------------------------------------   Out of Stamina
                //staminaLowSound.Play();
                return;
            }

            StartCoroutine(DoDodge(GetStateSettings(State.Dodging).duration));
        }

        private IEnumerator DoDodge(float duration)
        {
            state = State.Dodging;
            BlockMovementInput = true;

            if (Stamina != null) Stamina.Use(GetStateSettings(state).staminaDrainAmount);
            if (dust != null) dust.Play();

            // TODO: Move charge interruption to PlayerController
            if (Character.Controller.attacking != null)
                Character.Controller.attacking.EndCharge(false);
            
            dodgeEvents.OnDodgeStarted.Invoke();
            if (Input == Vector2.zero)
            {
                NeutralDodge();
            }
            else
            {
                DirectionalDodge(Input);
            }

            yield return new WaitForSeconds(duration);
            // TODO: Kan niet rennen
            state = State.Stopped;
            BlockMovementInput = false;
            dodgeEvents.OnDodgeCompleted.Invoke();
            ForceReadMoveInput();
            if (dust != null) dust.Stop();
        }

        private void NeutralDodge()
        {
            switch (defaultDodgeDirection)
            {
                case DefaultDodgeDirection.Backward:
                    DirectionalDodge(FacingDirection * -1f);
                    break;
                case DefaultDodgeDirection.ToCamera:
                    DirectionalDodge(new Vector2(0f, -1f));
                    break;
            }
        }

        private void DirectionalDodge(Vector2 direction)
        {
            DodgeDirection = direction.normalized;
        }

        public void UpdateDirectionIndicator(Vector2 facingDirection)
        {
            if (Direction == null)
            {
                Debug.LogWarning("Direction indicator is null. Adding...", Direction);
                Direction = new GameObject("Direction Indicator").AddComponent<Direction>();
                Direction.transform.parent = transform;
                Direction.transform.position = Vector3.zero;
                return;
            }
            Direction.UpdatePosition(new Vector3(facingDirection.x, 0f, facingDirection.y));
        }

        public bool LockFacingDirection = false;
        public void UpdateFacingDirection(Vector2 direction)
        {
            if (LockFacingDirection)
                return;

            FacingDirection = direction;
            UpdateDirectionIndicator(FacingDirection);
            if (Character.Animator == null)
                return;
            Character.Animator.SetFloat("Hor", direction.x);
            Character.Animator.SetFloat("Vert", direction.y);
        }
    }
}