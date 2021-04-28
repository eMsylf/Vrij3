using BobJeltes.Extensions;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System.Collections;
using UnityEngine.Events;
using Gyrus;

namespace RanchyRats.Gyrus
{
    [RequireComponent(typeof(Rigidbody))]
    public partial class Movement : CharacterComponent
    {
        public bool BlockMovementInput = false;
        public bool LockPosition = false;
        public bool LockFacingDirection = false;
        [Tooltip("If the movement is being controlled by a NavMeshAgent component, the movement animation is still updated through this component but the movement is not applied to the rigidbody.")]
        public bool MovementByNavMeshAgent = false;
        internal Vector2 Input = Vector2.zero;
        public Transform RotatingBody;
        public Transform StaticVisualBody;
        internal Vector2 FacingDirection = Vector2.up;
        internal Vector3 playerMovement
        {
            get
            {
                // If the player is currently dodging and no input was pressed at the beginning of the dodge, return the selected default dodge
                if (state == State.Dodging && Input == Vector2.zero)
                {
                    if (defaultDodgeDirection == DefaultDodgeDirection.ToCamera) return new Vector2(0f, -1f) * CurrentStateSettings.speed;
                    return -RotatingBody.forward * CurrentStateSettings.speed;
                }
                // Otherwise, return the regular movement
                return Input.XYToXZ() * CurrentStateSettings.speed;
            }
        }

        internal enum DefaultDodgeDirection { Backward, ToCamera }
        [SerializeField] internal DefaultDodgeDirection defaultDodgeDirection = DefaultDodgeDirection.Backward;

        [System.Serializable]
        public struct Events
        {
            public UnityEvent OnDodgeStarted;
            public UnityEvent OnDodgeCompleted;
        }

        // TODO: Use charachter stamina instead of separate reference
        public CharacterStatistic Stamina;

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

        public MovementStateSettings CurrentStateSettings
        {
            get
            {
                return GetStateSettings(state);
            }
        }
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
            if (MovementByNavMeshAgent)
                return;
            // TODO: De camera kan worden doorgegeven vanuit... ergens
            Rigidbody.MovePosition(Rigidbody.position + playerMovement.ConvertToObjectRelative(Camera.main.transform, true, true) * Time.fixedDeltaTime);
        }

        public bool ContinuousInputRead = true;

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
            if (BlockMovementInput)
            {
                //Debug.LogWarning("Tried to move while input was not accepted");
                return;
            }

            Input = input;
            if (Input == Vector2.zero)
            {
                state = State.Stopped;
                return;
            }

            if (!LockPosition && state == State.Stopped)
            {
                if (Character.Animator != null) Character.Animator.SetBool("IsWalking", Input != Vector2.zero);
                state = State.Walking;
            }
            SetFacingDirection(Input);
        }

        public void Stop()
        {
            Input = Vector2.zero;
            state = State.Stopped;
            if (Character.Animator != null)
                Character.Animator.SetBool("IsWalking", false);
        }

        public void StartRunning()
        {
            if (state == State.Dodging) return;
            state = State.Running;
            if (RunningSettings.drainsStamina && Stamina != null && Stamina.IsEmpty(false))
                return;
            runStaminaDrainTimeRemaining = GetStateSettings(State.Running).staminaDrainInterval;
            Character.Animator.SetFloat("RunningMultiplier", runningAnimationMultiplier);
        }

        public void StopRunning()
        {
            if (state == State.Running) state = State.Walking;
            Character.Animator.SetFloat("RunningMultiplier", 1f);
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
                Stamina.Use(GetStateSettings(State.Running).staminaDrainAmount);

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

            if (Stamina != null) Stamina.Use(GetStateSettings(state).staminaDrainAmount);
            if (dust != null) dust.Play();

            if (Character.Controller.attacking != null)
                Character.Controller.attacking.EndCharge(false);

            BlockMovementInput = true;

            dodgeEvents.OnDodgeStarted.Invoke();
            yield return new WaitForSeconds(duration);
            dodgeEvents.OnDodgeCompleted.Invoke();

            BlockMovementInput = false;
            ForceReadMoveInput();
            if (dust != null) dust.Stop();
        }

        public void SetFacingDirection(Vector2 direction)
        {
            if (LockFacingDirection)
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