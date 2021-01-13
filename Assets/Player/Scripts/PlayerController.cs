﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Combat;
using BobJeltes.Extensions;

public class PlayerController : Fighter
{
    #region Julia Added
    //Hey Julia here, I'm just throwing extra code things in here for now and will add comments where I also added something.
    public ParticleSystem dust;
    public void CreateDust()
    {
        dust.Play();
    }

    public FMODUnity.StudioEventEmitter attack1Sound;
    public FMODUnity.StudioEventEmitter attack2Sound;
    public FMODUnity.StudioEventEmitter attack3Sound;


    public FMODUnity.StudioEventEmitter getHitSound;

    public FMODUnity.StudioEventEmitter footstepsSound;
    public FMODUnity.StudioEventEmitter dodgeSound;

    public FMODUnity.StudioEventEmitter staminaLowSound;
    public FMODUnity.StudioEventEmitter staminaRegainSound;
    public FMODUnity.StudioEventEmitter healthLowSound;

    public void PlayAttackSound(int index) {
        if (index == 0) attack1Sound.Play();
        else if (index == 1) attack2Sound.Play();
        else if (index == 2)attack3Sound.Play();

    }


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

    static PlayerController instance;
    public static PlayerController Instance
    {
        get
        {
            if (instance == null)
            {
                // Search for existing instance.
                instance = (PlayerController)FindObjectOfType(typeof(PlayerController));

                // Create new instance if one doesn't already exist.
                //if (instance == null)
                //{
                //    // Need to create a new GameObject to attach the singleton to.
                //    var singletonObject = new GameObject();
                //    instance = singletonObject.AddComponent<PlayerController>();
                //    singletonObject.name = typeof(PlayerController).ToString() + " (Singleton)";

                //    // Make instance persistent.
                //    DontDestroyOnLoad(singletonObject);
                //}
            }

            return instance;
        }
    }

    [SerializeField] private Vector3 respawnPoint = new Vector3();
    public Vector3 RespawnPoint
    {
        get
        {
            if (OverrideRespawn == null)
                return respawnPoint;
            return OverrideRespawn.position;
        }
    }
    public Transform OverrideRespawn;

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

    public void Respawn()
    {
        Instance._Respawn();
    }

    private void _Respawn()
    {
        transform.position = RespawnPoint;
    }

    private void OnDisable()
    {
        //Debug.Log("Player disabled");
        Controls.Game.Disable();
        UnsubControls();

        OnDisableTasks();

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
        Controls.Game.Movement.performed += _ => SetMoveInput(_.ReadValue<Vector2>());
        Controls.Game.Movement.canceled += _ => Stop();

        Controls.Game.Dodge.performed += _ => AttemptDodge();

        Controls.Game.Attack.performed += _ => AttemptAttackCharge();
        Controls.Game.Attack.canceled += _ => attacking.CompleteCharge();

        Controls.Game.LockOn.performed += _ => targeting.LockOn(transform.position);

        Controls.Game.Run.started += _ => Run(true);
        Controls.Game.Run.canceled += _ => Run(false);

        controlsSubscribed = true;
    }

    private void UnsubControls()
    {
        if (!controlsSubscribed)
            return;
        Controls.Game.Movement.performed -= _ => SetMoveInput(_.ReadValue<Vector2>());
        Controls.Game.Movement.canceled -= _ => Stop();
        
        Controls.Game.Dodge.performed -= _ => AttemptDodge();
        
        Controls.Game.Attack.performed -= _ => AttemptAttackCharge();
        Controls.Game.Attack.canceled -= _ => attacking.CompleteCharge();
        
        Controls.Game.LockOn.performed -= _ => targeting.LockOn(transform.position);
        
        Controls.Game.Run.started -= _ => Run(true);
        Controls.Game.Run.canceled -= _ => Run(false);

        controlsSubscribed = false;
    }

    private void FixedUpdate()
    {
        Vector3 playerMovement = movement.GetTopDownMovement(movement.state) * movement.GetSpeedModifier(movement.state);
        Rigidbody.MovePosition(Rigidbody.position + playerMovement.ConvertToObjectRelative(Camera.main.transform, true, true) * Time.fixedDeltaTime);
    }

    public override void Update()
    {
        base.Update();
        ManageRuningStaminaDrain(movement.running);
        switch (movement.state)
        {
            case Movement.State.Idle:
            case Movement.State.Dodging:
            case Movement.State.Hit:
            case Movement.State.Stunned:
            case Movement.State.Disabled:
                break;
            case Movement.State.Moving:
                if (!footstepsSound.IsPlaying())
                    footstepsSound.Play(); //----------------------------------- Footsteps :)
                break;
        }
    }

    public override void Die()
    {
        InterruptAttackCharge();
        GameManager.Instance.PlayerDeath(this);
        //----------------------------------------------------------- Player dies
        if (dieSound != null)
            dieSound.Play();
        base.Die();
    }

    public void InterruptAttackCharge()
    {
        StopCoroutine(attacking.DoCharge());
        if (attacking.slowmotionInitiated) TimeManager.Instance.StopSlowmotion();
        attacking.InterruptCharge();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == Rigidbody)
        {
            Debug.Log("Hit self");
            return;
        }
    }

    public Direction Direction;
    public void UpdateDirectionIndicator()
    {
        Direction.DirectionIndicator.localPosition = new Vector3(movement.FacingDirection.x, 0f, movement.FacingDirection.y);
    }

    #region Movement
    [System.Serializable]
    public class Movement
    {
        public float Speed = 1f;
        public float DodgeSpeed = 2f;
        public float DodgeDuration = 1f;
        public bool AcceptMovementInput = true;
        public float RunSpeedMultiplier = 1.5f;
        [Tooltip("The amount of time it takes for running to take a point of stamina. (seconds)")]
        public float RunStaminaDrainTime = 1f;
        internal float runStaminaDrainTime = 0f;
        internal bool running = false;
        internal Vector2 Input;
        internal Vector2 FacingDirection = new Vector2(0f, 1f);
        internal enum DefaultDodgeDirection { Backward, ToCamera} 
        [SerializeField] internal DefaultDodgeDirection defaultDodgeDirection = DefaultDodgeDirection.Backward;
        internal Vector2 DodgeDirection;

        public enum State
        {
            Idle = default,
            Moving,
            Dodging,
            Hit,
            Stunned,
            Disabled
        }
        public State state = default;

        #region It would be nice if I could combine these somehow
        internal Vector3 GetTopDownMovement(State state)
        {
            Vector3 movement = Vector3.zero;
            switch (state)
            {
                case State.Moving:
                    movement = new Vector3(Input.x, 0f, Input.y);
                    break;
                case State.Dodging:
                    movement = new Vector3(DodgeDirection.x, 0f, DodgeDirection.y);
                    
                    break;
            }
            return movement;
        }

        internal float GetSpeedModifier(State state)
        {
            switch (state)
            {
                case State.Moving:
                    float speed = Speed * (running ? RunSpeedMultiplier : 1f);
                    return speed;
                case State.Dodging:
                    return DodgeSpeed;
                case State.Hit:
                case State.Stunned:
                case State.Idle:
                case State.Disabled:
                default:
                    return 0f;
            }
        }
        #endregion
    }
    public Movement movement;

    private void SetMoveInput(Vector2 input)
    {
        if (!movement.AcceptMovementInput)
        {
            //Debug.LogWarning("Tried to move while input was not accepted");
            return;
        }
        //Debug.Log("Input: " + input);

        switch (movement.state)
        {
            case Movement.State.Dodging:
            case Movement.State.Hit:
            case Movement.State.Stunned:
            case Movement.State.Disabled:
                Debug.Log("Player can't move while " + movement.state.ToString());
                return;
            case Movement.State.Idle:
                movement.state = Movement.State.Moving;
                break;
            case Movement.State.Moving:
                break;
        }
        movement.Input = input;
        Animator.SetBool("IsWalking", true);
        UpdateFacingDirection(movement.Input);
    }

    private void Stop()
    {
        //Debug.Log("Stop!");
        movement.Input = Vector2.zero;
        //UpdateAnimatorDirection(Direction.UpdateLookDirection(MovementInput));
        movement.state = Movement.State.Idle;
        Animator.SetBool("IsWalking", false);
        footstepsSound.Stop(); //----------------------------------- Footstops :)
   
    }

    private void Run(bool enabled)
    {
        if (enabled && Stamina.m_value <= 0)
            return; 

        movement.running = enabled;
        Stamina.allowRecovery = !enabled;
        //Debug.Log("Running: " + enabled, this);
        if (enabled)
            movement.runStaminaDrainTime = movement.RunStaminaDrainTime;
    }

    private void ManageRuningStaminaDrain(bool running)
    {
        if (!running)
            return;

        movement.runStaminaDrainTime -= Time.deltaTime;

        if (movement.runStaminaDrainTime <= 0f)
        {
            movement.runStaminaDrainTime = movement.RunStaminaDrainTime;
            Stamina.Use(1);

            if (Stamina.m_value <= 0)
            {
                Run(false);
            }
        }
    }

    private void AttemptDodge()
    {
        switch (movement.state)
        {
            // Dodging is allowed when
            case Movement.State.Idle:
            case Movement.State.Moving:
                break;
            // Dodging is not allowed when
            case Movement.State.Dodging:
            case Movement.State.Hit:
            case Movement.State.Stunned:
            case Movement.State.Disabled:
                return;
        }

        switch (attacking.state)
        {
            case Attacking.State.Ready:
            case Attacking.State.OnCooldown:
            case Attacking.State.Disabled:
            case Attacking.State.Charging:
                break;
            case Attacking.State.Attacking:
                return;
        }

        if (Stamina.m_value <= 0)
        {
            //Debug.Log("Insufficient stamina to dodge");
            //-----------------------------------------------   Out of Stamina
            //staminaLowSound.Play();
            return;
        }
        Stamina.Use(1);

        //attacking.state = Attacking.State.Disabled;

        StartCoroutine(Dodge(movement.DodgeDuration));
        CreateDust(); //-------------------------------------   Creates dust, added by Julia :^)
    }

    private IEnumerator Dodge(float duration)
    {
        movement.state = Movement.State.Dodging;
        movement.AcceptMovementInput = false;
        // ---------------------------------------------    Dodge sound
        dodgeSound.Play();

        if (movement.Input == Vector2Int.zero)
        {
            NeutralDodge();
        }
        else
        {
            DirectionalDodge(movement.Input);
        }

        yield return new WaitForSeconds(duration);
        movement.state = Movement.State.Idle;
        movement.AcceptMovementInput = true;
        UpdateMoveInput();
    }

    private void NeutralDodge()
    {
        switch (movement.defaultDodgeDirection)
        {
            case Movement.DefaultDodgeDirection.Backward:
                DirectionalDodge(movement.FacingDirection * -1f);
                break;
            case Movement.DefaultDodgeDirection.ToCamera:
                DirectionalDodge(new Vector2(0f, -1f));
                break;
        }
    }

    private void DirectionalDodge(Vector2 direction)
    {
        movement.DodgeDirection = direction.normalized; 
        //Debug.Log("Input detected. Dodge direction = " + movement.DodgeDirection.ToString());

    }

    private void UpdateMoveInput()
    {
        Vector2 readMovement = Controls.Game.Movement.ReadValue<Vector2>();
        if (readMovement != Vector2.zero)
        {
            SetMoveInput(readMovement);
            // ---------------------------------------------    Footsteps Bob help werkt nie goe :(
            footstepsSound.Play();
        }

        else
        {
            //footstepsSound.Stop();
            Stop();
        }
    }

    public void UpdateFacingDirection(Vector2 direction)
    {
        movement.FacingDirection = direction;
        //TODO: Do dit niet wanneer de speler aan het aanvallen is
        UpdateDirectionIndicator();
        if (Animator == null)
            return;
        Animator.SetFloat("Hor", direction.x);
        Animator.SetFloat("Vert", direction.y);
    }
    #endregion

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

        if (Stamina.m_value <= 0)
        {
            Debug.Log("Not enough stamina to attack");
            //-------------------------------------------------- Out of stamina
            //Julia: Hey Bob, is dit komt ook voor als de player dodged, is het niet slimmer die 2 te combineren?
            staminaLowSound.Play();
            return;
        }
        Stamina.Use(1);

        StartCoroutine(attacking.DoCharge());
        Stamina.allowRecovery = false;
    }

    public void AddNoChargeZone(GameObject zone)
    {
        if (Instance == null)
            return;
        if (Instance.attacking == null)
            return;
            Instance.attacking.AddChargingProhibitor(zone);
    }

    public void RemoveNoChargeZone(GameObject zone)
    {
        if (Instance == null)
            return;
        if (Instance.attacking == null)
            return;
            Instance.attacking.RemoveChargingProhibitor(zone);
    }

    public void OnAttack()
    {
        Animator.SetFloat("AttackCharge", attacking.latestCharge);
        Animator.SetTrigger("Attack");
        attacking.WeaponAnimator.SetFloat("AttackCharge", attacking.latestCharge);
        attacking.WeaponAnimator.SetTrigger("Attack");

        Stop();
        //Debug.Log("OnAttack, set movement input to false");
        movement.AcceptMovementInput = false;
        attacking.state = Attacking.State.Attacking;
        movement.state = Movement.State.Disabled;
        //Animator.ResetTrigger("Attack");
        Stamina.allowRecovery = false;

        //-------------------------------------------------- Attack sound, according to latestCharge 2 to 3/4
        //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" + attacking.latestCharge);
        //if (attacking.latestCharge <= 0) attack1Sound.Play();
        //else if (attacking.latestCharge <= 2) attack2Sound.Play();
        //else attack3Sound.Play();
    }

    public void OnAttackEnd()
    {
        Instance.OnAttackEndInstance();
        //-------------------------------------------------- stop attack sound
    }

    private void OnAttackEndInstance()
    {
        //Debug.Log("On attack end from instance. Accept movement input again.", this.gameObject);
        attacking.state = Attacking.State.Ready;
        movement.state = Movement.State.Idle;
        movement.AcceptMovementInput = true;
        Stamina.allowRecovery = true;
        // Get walking direction at end of attack 
        UpdateMoveInput();
    }

    #endregion

    #region Targeting
    [System.Serializable]
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
        Handles.color = Color.white;
        Handles.DrawLine(transform.position, RespawnPoint);
        Gizmos.DrawIcon(RespawnPoint, "Player Spawn Pos");
    }
#endif
}
