using BobJeltes.Extensions;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System.Collections;
using UnityEngine.Events;
using Gyrus;
using BobJeltes.Attributes;
using System;

namespace RanchyRats.Gyrus
{
    [RequireComponent(typeof(Rigidbody))]
    public partial class Movement : CharacterComponent
    {
        public Transform RotatingBody;
        public Transform StaticVisualBody;

        [Header("Constraints")]
        public Constraints constraints;
        [Flags] public enum Constraints
        {
            None = 0,
            LockInput = 1,
            Position = 2,
            Rotation = 4
        }
        [Header("Movement type")]
        [Tooltip("If the movement is being controlled by a NavMeshAgent component, the movement animation is still updated through this component but the movement is not applied to the rigidbody.")]
        public bool NavMeshAgentControlled = false;
        [ShowIf("NavMeshAgentControlled", false)]
        public bool MovePosition = true;
        [ShowIf("MovePosition", false)]
        public ForceMode forceMode = ForceMode.Acceleration;
        [Tooltip("Temporary fix for accurately updating the character's facing direction")]
        public bool ContinuousInputRead = true;
        internal Vector2 Input = Vector2.zero;
        internal Vector2 FacingDirection = Vector2.up;
        internal Vector3 playerMovement
        {
            get
            {
                // If the player is currently dodging and no input was pressed at the beginning of the dodge, return the selected default dodge
                if (state == State.Dodging && Input == Vector2.zero)
                {
                    if (defaultDodgeDirection == DefaultDodgeDirection.ToCamera) return new Vector2(0f, -1f) * Speed;
                    return -RotatingBody.forward * Speed;
                }
                // Otherwise, return the regular movement
                return Input.XYToXZ() * Speed;
            }
        }

        [System.Serializable]
        public struct DodgeEvents
        {
            public UnityEvent OnDodgeStarted;
            public UnityEvent OnDodgeCompleted;
        }

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

        [Header("Settings")]
        public float BaseSpeed = 1f;
        public float Speed => BaseSpeed * CurrentStateSettings.speed;
        public enum State
        {
            Stopped = default,
            Walking,
            Running,
            Dodging
        }
        public State state = default;
        // TODO: Zou chill zijn als deze als een tabel weergegeven zouden kunnen worden
        public MovementStateSettings StoppedSettings = new MovementStateSettings(0);
        public MovementStateSettings WalkingSettings = new MovementStateSettings(1f) { animationParameter = "Walking" };
        public MovementStateSettings RunningSettings = new MovementStateSettings(1.5f);
        private float runStaminaDrainTimeRemaining;
        public MovementStateSettings DodgingSettings = new MovementStateSettings(2f, true, .3f, State.Stopped, true, 1, 0);
        internal enum DefaultDodgeDirection { Backward, ToCamera }
        [SerializeField] internal DefaultDodgeDirection defaultDodgeDirection = DefaultDodgeDirection.Backward;
        public DodgeEvents dodgeEvents;

        public MovementStateSettings CurrentStateSettings => GetStateSettings(state);
        public MovementStateSettings GetStateSettings(State state)
        {
            switch (state)
            {
                default:
                case State.Stopped: return StoppedSettings;
                case State.Walking: return WalkingSettings;
                case State.Running: return RunningSettings;
                case State.Dodging: return DodgingSettings;
            }
        }

        private void OnEnable()
        {
            state = State.Stopped;
        }

        private void FixedUpdate()
        {
            if (NavMeshAgentControlled)
                return;
            // TODO: De camera kan worden doorgegeven vanuit... ergens
            if (MovePosition)
                Rigidbody.MovePosition(Rigidbody.position + playerMovement.ConvertToObjectRelative(Camera.main.transform, true, true));
            else
                Rigidbody.AddForce(playerMovement.ConvertToObjectRelative(Camera.main.transform, true, true), forceMode);
        }

        private void Update()
        {
            ManageRunningStaminaDrain();
            if (ContinuousInputRead) ForceReadMoveInput();
            UpdateVisualFacing();
        }

        // When an attack or dodge ends, the input from the player needs to be read again so the player will start to move if the player is holding the movement stick in a direction at the end of an attack.
        public void ForceReadMoveInput()
        {
            if (Character.Controller.PlayerController == null)
                return;

            Vector2 input = (Character.Controller.PlayerController).Controls.Game.Movement.ReadValue<Vector2>();
            if (input == Vector2.zero)
            {
                Stop();
                return;
            }

            SetMoveInput(input);
            bool run = (Character.Controller.PlayerController).Controls.Game.Run.phase == UnityEngine.InputSystem.InputActionPhase.Started;
            if (run)
                StartRunning();
        }

        public void SetMoveInput(Vector2 input)
        {
            if (constraints.HasFlag(Constraints.LockInput))
                return;

            Input = input;
            if (Input == Vector2.zero)
            {
                state = State.Stopped;
                return;
            }
            //Allow movement bool/flag?
            if (!constraints.HasFlag(Constraints.Position) && state == State.Stopped)
            {
                if (Character.Animator != null) Character.Animator.SetBool(WalkingSettings.animationParameter, Input != Vector2.zero);
                state = State.Walking;
            }
            SetFacingDirection(Input);
        }

        public void Stop()
        {
            Input = Vector2.zero;
            state = State.Stopped;
            if (Character.Animator != null)
                Character.Animator.SetBool(WalkingSettings.animationParameter, false);
        }

        public void StartRunning()
        {
            if (state == State.Dodging) return;
            state = State.Running;
            if (RunningSettings.drainsStamina && Character.stamina != null && Character.stamina.IsEmpty(false))
                return;
            runStaminaDrainTimeRemaining = GetStateSettings(State.Running).staminaDrainInterval;
            // Running animation multiplier is set by dividing the walking speed by the running speed.
            Character.Animator.SetFloat("MovementSpeed", RunningSettings.speed/WalkingSettings.speed);
        }

        public void StopRunning()
        {
            if (state == State.Running) state = State.Walking;
            Character.Animator.SetFloat("MovementSpeed", 1f);
        }

        private void ManageRunningStaminaDrain()
        {
            if (state != State.Running) return;
            if (!RunningSettings.drainsStamina)
                return;
            // TODO: Implement stamina drain settings
            runStaminaDrainTimeRemaining -= Time.deltaTime;

            if (runStaminaDrainTimeRemaining <= 0f)
            {
                runStaminaDrainTimeRemaining = GetStateSettings(State.Running).staminaDrainInterval;
                Character.stamina.Use(GetStateSettings(State.Running).staminaDrainAmount);

                if (Character.stamina.IsEmpty(true))
                {
                    StopRunning();
                }
            }
        }

        public void AttemptDodge()
        {
            switch (state)
            {
                // Dodging is not allowed when
                case State.Dodging:
                    return;
            }

            if (DodgingSettings.drainsStamina && Character.stamina != null && Character.stamina.IsEmpty(true))
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

            if (Character.stamina != null) Character.stamina.Use(GetStateSettings(state).staminaDrainAmount);

            if (Character.Controller.attacking != null)
                Character.Controller.attacking.EndCharge(false);

            constraints |= Constraints.LockInput;

            dodgeEvents.OnDodgeStarted.Invoke();
            yield return new WaitForSeconds(duration);
            dodgeEvents.OnDodgeCompleted.Invoke();

            constraints ^= Constraints.LockInput;
            
            ForceReadMoveInput();
        }

        public void SetFacingDirection(Vector2 direction)
        {
            if (constraints.HasFlag(Constraints.Rotation))
                return;

            FacingDirection = direction;
            Vector3 cameraRelativeDirection = direction.XYToXZ().ConvertToObjectRelative(Camera.main.transform, true);
            RotatingBody.rotation = Quaternion.LookRotation(cameraRelativeDirection);
        }

        public void UpdateVisualFacing()
        {
            Vector3 worldspaceDirection = StaticVisualBody.InverseTransformVector(RotatingBody.transform.forward);
            if (Character.Animator == null)
                return;
            Character.Animator.SetFloat("Hor", worldspaceDirection.x);
            Character.Animator.SetFloat("Vert", worldspaceDirection.z);
        }
    }
}