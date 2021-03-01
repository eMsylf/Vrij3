using BobJeltes.Extensions;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using Combat;
using System.Collections;
using UnityEngine.Events;
using RanchyRats.Gyrus;

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
    public bool AcceptMovementInput = true;
    internal Vector2 Input;
    internal Vector2 FacingDirection = new Vector2(0f, 1f);
    internal enum DefaultDodgeDirection { Backward, ToCamera }
    [SerializeField] internal DefaultDodgeDirection defaultDodgeDirection = DefaultDodgeDirection.Backward;
    internal Vector2 DodgeDirection;

    public Stat Stamina;

    //private CharacterController

    public Animator Animator;
    public Direction Direction;

    public struct Sounds
    {
        public StudioEventEmitter Footstep;
        public StudioEventEmitter Dodge;
    }
    public Sounds sounds;

    
    [Min(0)]
    public float footstepInterval = .5f;
    [Min(0)]
    public float footstepIntervalRunning = .25f;
    private float timeBeforeNextFootstep;

    //Hey Julia here, I'm just throwing extra code things in here for now and will add comments where I also added something.
    public ParticleSystem dust;
    public void CreateDust()
    {
        dust.Play();
    }


#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    Rigidbody rigidbody;
    Rigidbody Rigidbody
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
        Idle = default,
        Walking,
        Running,
        Dodging
    }
    public State state = default;

    private void FixedUpdate()
    {
        Vector3 playerMovement = GetTopDownMovement(Input, state) * GetSettings(state).speed;
        Rigidbody.MovePosition(Rigidbody.position + playerMovement.ConvertToObjectRelative(Camera.main.transform, true, true) * Time.fixedDeltaTime);
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
            case State.Dodging:
                break;
            case State.Walking:
                ManageFootstepSound();
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

    [System.Serializable]
    public struct MovementStateSettings
    {
        public float speed;
        public bool expires;
        public float duration;
        public State nextState;
        public bool drainsStamina;
        public float staminaDrainAmount;
        public float staminaDrainInterval;

        public MovementStateSettings(float speed, bool expires = false, float duration = 0f, State nextState = State.Idle, bool drainsStamina = false, float staminaDrainAmount = 0f, float staminaDrainInterval = 1f)
        {
            this.speed = speed;
            this.expires = expires;
            this.duration = duration;
            this.nextState = nextState;
            this.drainsStamina = drainsStamina;
            this.staminaDrainAmount = staminaDrainAmount;
            this.staminaDrainInterval = staminaDrainInterval;
        }
    }
    // TODO: Zou chill zijn als deze als een tabel weergegeven zouden kunnen worden
    public MovementStateSettings IdleSettings = new MovementStateSettings(0);
    public MovementStateSettings WalkingSettings = new MovementStateSettings(1f);
    public MovementStateSettings RunningSettings = new MovementStateSettings(1.5f);
    public MovementStateSettings DodgingSettings = new MovementStateSettings(2f, true, .3f, State.Idle, true, 1, 0);

    public MovementStateSettings GetSettings(State state)
    {
        switch (state)
        {
            case State.Idle:
            default:
                return IdleSettings;
            case State.Walking:
                return WalkingSettings;
            case State.Running:
                return RunningSettings;
            case State.Dodging:
                return DodgingSettings;
        }
    }
    
    public void SetMoveInput(Vector2 input)
    {
        if (!AcceptMovementInput)
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
            case State.Idle:
                state = State.Walking;
                break;
            case State.Walking:
                break;
        }
        Input = input;
        Animator.SetBool("IsWalking", true);
        UpdateFacingDirection(Input);
    }

    public void Stop()
    {
        //Debug.Log("Stop!");
        Input = Vector2.zero;
        //UpdateAnimatorDirection(Direction.UpdateLookDirection(MovementInput));
        state = State.Idle;
        Animator.SetBool("IsWalking", false);
        sounds.Footstep.Stop(); //----------------------------------- Footstops :)

    }

    public float runningAnimationMultiplier = 1.5f;
    private float runStaminaDrainTime;

    public void StartRunning()
    {
        if (Stamina.IsEmpty(false))
            return;

        state = State.Running;
        Animator.SetFloat("RunningMultiplier", runningAnimationMultiplier);
        Stamina.allowRecovery = false;
        timeBeforeNextFootstep = 0f;
        //Debug.Log("Running: " + enabled, this);
        runStaminaDrainTime = GetSettings(State.Running).staminaDrainInterval;
    }

    public void StopRunning()
    {
        switch (state)
        {
            case State.Idle:
            default:
            case State.Walking:
            case State.Dodging:
                break;
            case State.Running:
                state = State.Walking;
                break;
        }
        Animator.SetFloat("RunningMultiplier", 1f);
    }

    private void ManageRunningStaminaDrain()
    {
        // TODO: Implement stamina drain settings
        runStaminaDrainTime -= Time.deltaTime;

        if (runStaminaDrainTime <= 0f)
        {
            runStaminaDrainTime = GetSettings(State.Running).staminaDrainInterval;
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
            case State.Idle:
            case State.Walking:
            case State.Running:
                break;
            // Dodging is not allowed when
            case State.Dodging:
                return;
        }

        if (Stamina.IsEmpty(true))
        {
            //Debug.Log("Insufficient stamina to dodge");
            //-----------------------------------------------   Out of Stamina
            //staminaLowSound.Play();
            return;
        }

        StartCoroutine(Dodge(GetSettings(State.Dodging).duration));
    }

    public UnityEvent OnDodgeStarted;
    public UnityEvent OnDodgeCompleted;

    private IEnumerator Dodge(float duration)
    {
        state = State.Dodging;
        Stamina.Use(1);
        CreateDust();
        AcceptMovementInput = false;
        
        sounds.Dodge.Play();
        // TODO: Move charge interruption to PlayerController
        OnDodgeStarted.Invoke();
        //attacking.InterruptCharge();
        if (Input == Vector2.zero)
        {
            NeutralDodge();
        }
        else
        {
            DirectionalDodge(Input);
        }

        yield return new WaitForSeconds(duration);
        state = State.Idle;
        AcceptMovementInput = true;
        OnDodgeCompleted.Invoke();
        // TODO: Update movement input
        //UpdateMoveInput();
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
        //Debug.Log("Input detected. Dodge direction = " + DodgeDirection.ToString());

    }


    public void UpdateDirectionIndicator(Vector2 facingDirection)
    {
        Direction.DirectionIndicator.localPosition = new Vector3(facingDirection.x, 0f, facingDirection.y);
    }

    public bool LockFacingDirection = false;
    public void UpdateFacingDirection(Vector2 direction)
    {
        if (LockFacingDirection)
            return;

        FacingDirection = direction;
        //TODO: Doe dit niet wanneer de speler aan het aanvallen is
        UpdateDirectionIndicator(FacingDirection);
        if (Animator == null)
            return;
        Animator.SetFloat("Hor", direction.x);
        Animator.SetFloat("Vert", direction.y);
    }

    public void ManageFootstepSound()
    {
        if (sounds.Footstep == null)
            return;

        if (timeBeforeNextFootstep > 0f)
        {
            timeBeforeNextFootstep -= Time.deltaTime;
            return;
        }

        sounds.Footstep.Play();
        if (state == State.Running)
            timeBeforeNextFootstep = footstepIntervalRunning;
        else
            timeBeforeNextFootstep = footstepInterval;
    }
}
