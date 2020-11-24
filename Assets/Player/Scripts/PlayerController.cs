using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Combat;
using UnityEngine.Audio;

public class PlayerController : Fighter
{
    #region Julia Added
    //Hey Julia here, I'm just throwing extra code things in here for now and will add comments where I also added something.
    public ParticleSystem dust;
    public void CreateDust(){
        dust.Play();
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

    new Rigidbody rigidbody;
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
                if (instance == null)
                {
                    // Need to create a new GameObject to attach the singleton to.
                    var singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<PlayerController>();
                    singletonObject.name = typeof(PlayerController).ToString() + " (Singleton)";

                    // Make instance persistent.
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return instance;
        }
    }

    private Vector3 SpawnPos;
    [Tooltip("If unticked, the player's position will be saved as its new spawn position the next time the player is enabled.")]
    public bool SpawnPosSet = false;

    private void OnEnable()
    {
        //Debug.Log("Player enabled");
        if (!SpawnPosSet)
        {
            SpawnPos = transform.position;
            SpawnPosSet = true;
        }
        else
        {
            transform.position = SpawnPos;
        }
        Controls.Game.Enable();
        SubscribeControls();

        attacking.attackLaunched += () => OnAttack();
        attacking.attackEnd += () => OnAttackEnd();

        OnEnableTasks();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        //Debug.Log("Player disabled");
        Controls.Game.Disable();
        UnsubControls();

        attacking.attackLaunched -= () => OnAttack();
        attacking.attackEnd -= () => OnAttackEnd();

        OnDisableTasks();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
        controlsSubscribed = false;
    }

    private void FixedUpdate()
    {
        Vector3 playerMovement = movement.GetTopDownMovement(movement.state) * movement.GetSpeedModifier(movement.state);
        
        Rigidbody.MovePosition(Rigidbody.position + ConvertToObjectRelative(Camera.main.transform, playerMovement, true, true) * Time.fixedDeltaTime);
    }

    public static Vector3 ConvertToObjectRelative(Transform reference, Vector3 vector3, bool flatten = false, bool normalize = false)
    {
        Vector3 referenceForward = reference.forward;
        if (flatten) 
            referenceForward.Scale(new Vector3(1, 0, 1));
        if (normalize)
            referenceForward.Normalize();
        Vector3 referenceRight = reference.right;
        Vector3 referenceRelativeVector3 = referenceForward * vector3.z + referenceRight * vector3.x;
        return referenceRelativeVector3;
    }

    public override void Die()
    {
        GameManager.Instance.PlayerDeath(this);
        //----------------------------------------------------------- Player dies
        base.Die();
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
                    return Speed;
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
            Debug.LogWarning("Tried to move while input was not accepted");
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

        if (!Stamina.AttemptUse())
        {
            //Debug.Log("Insufficient stamina to dodge");
            //-----------------------------------------------   Out of Stamina
            return;
        }

        attacking.state = Attacking.State.Disabled;

        StartCoroutine(Dodge(movement.DodgeDuration));
        CreateDust(); //-------------------------------------   Creates dust, added by Julia :^)
    }

    private IEnumerator Dodge(float duration)
    {
        movement.state = Movement.State.Dodging;
        movement.AcceptMovementInput = false;
        // ---------------------------------------------    Dodge sound
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
        attacking.state = Attacking.State.Ready;
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
            SetMoveInput(readMovement);
        // ---------------------------------------------    Footsteps

        else
        {
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
        public Slider ChargeSlider;
        public GameObject ChargeIndicators;
        public enum EChargeType
        {
            States,
            Slider,
            Both
        }
        public EChargeType ChargeType;
        public Animator WeaponAnimator;
        public Gradient ChargeZones;
        public Statistic ChargeIndicator;
        [Tooltip("Time it takes for the slider to fill up")]
        public float ChargeTime = 2f;
        [Tooltip("Time below which a charge will not be initiated")]
        public float ChargeTimeDeadzone = .1f;
        public bool ChargeEffectedBySlowdown = false;

        [Range(0f, 1f)]
        public float startSlowmotionAt = .5f;
        [Range(0f, 1f)]
        public float slowmotionFactor = .25f;

        internal bool allowCharging = true;
        internal float latestCharge;
        internal UnityAction attackLaunched;
        internal UnityAction attackEnd;

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
                //----------------------------------------- Attack 1 hit
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

        GameObject GetChargeObject()
        {
            GameObject obj;
            switch (ChargeType)
            {
                case EChargeType.Slider:
                    obj = ChargeSlider.gameObject;
                    break;
                default:
                    obj = ChargeIndicators.gameObject;
                    break;
            }
            if (obj == null)
            {
                Debug.LogWarning("Charge object " + ChargeType + " of Player is null!");
            }
            return obj;
        }

        GameObject[] GetChargeObjects()
        {
            switch (ChargeType)
            {
                case EChargeType.Slider:
                    return new GameObject[] { ChargeSlider.gameObject };
                default:
                    return new GameObject[] { ChargeIndicators.gameObject };
                case EChargeType.Both:
                    return new GameObject[] { ChargeSlider.gameObject, ChargeIndicators.gameObject };
            }
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
        public int chargeProhibitors = 0;
        public bool ChargingAllowed()
        {
            return chargeProhibitors == 0;
        }

        public void increaseChargingProhibitors()
        {
            Debug.Log("Increase charge prohibitors");
            chargeProhibitors++;
        }

        public void decreaseChargingProhibitors()
        {
            Debug.Log("Decrease charge prohibitors");
            chargeProhibitors--;
        }

        public IEnumerator DoCharge()
        {
            state = State.Charging;
            switch (ChargeType)
            {
                case EChargeType.Slider:
                    ChargeIndicator.SetCurrent(0, true);
                    break;
                default:
                    ChargeIndicator.SetCurrent(0, true);
                    break;
            }
            float chargeTime = 0f;
            float chargeTimeClamped = 0f;
            //Debug.Log("Start charge");
            while (chargeTime < ChargeTimeDeadzone)
            {
                yield return new WaitForEndOfFrame();
                chargeTime += Time.unscaledDeltaTime;
            }
            //Debug.Log("Charge deadzone passed");
            GetChargeObject().SetActive(true);
            bool slowmotionInitiated = false;
            
            int previousChargeState = 0;
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

                chargeTimeClamped = Mathf.Clamp01(chargeTime / ChargeTime);
                //Debug.Log("Chargetime clamped: " + chargeTimeClamped);

                int currentChargeState = GetChargeZoneIndex(chargeTimeClamped);
                
                switch (ChargeType)
                {
                    case EChargeType.Slider:
                        ChargeSlider.value = chargeTimeClamped;
                        ChargeIndicator.SetCurrent(currentChargeState + 1, false);
                        break;
                    case EChargeType.States:
                        if (currentChargeState != previousChargeState)
                        {
                            ChargeIndicator.SetCurrent(currentChargeState + 1);
                            previousChargeState = currentChargeState;
                        }
                        break;
                }
                if (!slowmotionInitiated && chargeTimeClamped > startSlowmotionAt)
                {
                    slowmotionInitiated = true;
                    TimeManager.Instance.DoSlowmotion(slowmotionFactor);
                }
            }
            TimeManager.Instance.StopSlowmotion();
            GetChargeObject().SetActive(false);

            //Launch(chargeTimeClamped);
            Launch(GetChargeZoneIndex(chargeTimeClamped));
        }

        public void ApplyChargeZoneColors()
        {
            GameObject chargeObject = GetChargeObject();
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

        if (!Stamina.AttemptUse())
        {
            Debug.Log("Not enough stamina to attack");
            //-------------------------------------------------- Out of stamina
            //Julia: Hey Bob, is dit komt ook voor als de player dodged, is het niet slimmer die 2 te combineren?
            return;
        }
        StartCoroutine(attacking.DoCharge());
        staminaRecharge.allow = false;
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
        staminaRecharge.allow = false;

        //-------------------------------------------------- Attack sound, according to latestCharge 2 to 3/4
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
        staminaRecharge.allow = true;
        // Get walking direction at end of attack 
        UpdateMoveInput();
    }

    public void NoChargeZoneIncrease()
    {
        Instance.attacking.increaseChargingProhibitors();
    }

    public void NoChargeZoneDecrease()
    {
        Instance.attacking.decreaseChargingProhibitors();
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
        Instance.StartCoroutine(GamepadRumble(duration));
    }

    public IEnumerator GamepadRumble(float duration)
    {
        Gamepad.current.SetMotorSpeeds(0.25f, 0.75f);
        yield return new WaitForSeconds(duration);
        Gamepad.current.SetMotorSpeeds(0f, 0f);
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Handles.color = targeting.RadiusColor;
        Handles.DrawWireDisc(transform.position, transform.up, targeting.Radius);
    }
#endif
}
