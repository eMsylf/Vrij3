using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Combat;
using UnityEditor;
using UnityEngine.UI;

public class PlayerController : Fighter
{
    public enum EState
    {
        Idle = default,
        Moving,
        Dodging,
        Attacking,
        Hit,
        Stunned
    }
    public EState State = default;

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

        EnableTasks();
    }

    private void OnDisable()
    {
        //Debug.Log("Player disabled");
        Controls.Game.Disable();
        UnsubControls();

        attacking.attackLaunched -= () => OnAttack();

        DisableTasks();
    }

    bool controlsSubscribed = false;
    private void SubscribeControls()
    {
        if (controlsSubscribed)
            return;
        Controls.Game.Movement.performed += _ => SetMoveInput(_.ReadValue<Vector2>());
        Controls.Game.Movement.canceled += _ => Stop();
        Controls.Game.Dodge.performed += _ => AttemptDodge();
        Controls.Game.Attack.performed += _ => AttemptAttack();
        Controls.Game.Attack.canceled += _ => attacking.charging = false;
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
        Controls.Game.Attack.performed -= _ => StartCoroutine(attacking.StartCharge());
        Controls.Game.Attack.canceled -= _ => attacking.charging = false;
        Controls.Game.LockOn.performed -= _ => targeting.LockOn(transform.position);
        controlsSubscribed = false;
    }

    private void FixedUpdate()
    {
        Vector3 forwardDirection = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1));
        Vector3 rightDirection = Camera.main.transform.right;

        Vector3 playerMovement = movement.GetTopDownMovement(State) * movement.GetSpeedModifier(State);
        Vector3 cameraRelativeMovement = forwardDirection * playerMovement.z + rightDirection * playerMovement.x;

        Rigidbody.MovePosition(Rigidbody.position + cameraRelativeMovement * Time.fixedUnscaledDeltaTime);
    }

    public override void Die()
    {
        GameManager.Instance.PlayerDeath(this);
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
        internal Vector2 DodgeDirection;

        #region It would be nice if I could combine these somehow
        internal Vector3 GetTopDownMovement(EState state)
        {
            Vector3 movement = Vector3.zero;
            switch (state)
            {
                case EState.Moving:
                    movement = new Vector3(Input.x, 0f, Input.y);
                    break;
                case EState.Dodging:
                    movement = new Vector3(DodgeDirection.x, 0f, DodgeDirection.y);
                    break;
            }
            return movement;
        }

        internal float GetSpeedModifier(EState state)
        {
            switch (state)
            {
                case EState.Moving:
                    return Speed;
                case EState.Dodging:
                    return DodgeSpeed;
                case EState.Attacking:
                case EState.Hit:
                case EState.Stunned:
                case EState.Idle:
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
            return;
        }

        switch (State)
        {
            case EState.Dodging:
            case EState.Attacking:
            case EState.Hit:
            case EState.Stunned:
                Debug.Log("Player can't move while " + State.ToString());
                return;
            case EState.Idle:
                State = EState.Moving;
                break;
        }
        movement.Input = input;
        Animator.SetBool("IsWalking", true);
        UpdateFacingDirection(movement.Input);
    }

    private void Stop()
    {
        if (!movement.AcceptMovementInput)
            return;
        //Debug.Log("Stop!");
        movement.Input = Vector2.zero;
        //UpdateAnimatorDirection(Direction.UpdateLookDirection(MovementInput));
        State = EState.Idle;
        Animator.SetBool("IsWalking", false);
    }

    private void AttemptDodge()
    {
        if (State == EState.Dodging)
        {
            return;
        }

        if (!Stamina.Use())
        {
            Debug.Log("Insufficient stamina to dodge");
            return;
        }

        StartCoroutine(Dodge(movement.DodgeDuration));
    }

    private IEnumerator Dodge(float duration)
    {
        State = EState.Dodging;
        movement.AcceptMovementInput = false;
        if (movement.Input == Vector2Int.zero)
        {
            movement.DodgeDirection = movement.FacingDirection * -1f;
            Debug.Log("No input detected. Dodge direction = " + movement.DodgeDirection.ToString());
        }
        else
        {
            movement.DodgeDirection = movement.Input;
            Debug.Log("Input detected. Dodge direction = " + movement.DodgeDirection.ToString());
        }

        yield return new WaitForSeconds(duration);

        State = EState.Idle;
        Vector2 directionAtEndOfDodge = Controls.Game.Movement.ReadValue<Vector2>();
        movement.AcceptMovementInput = true;
        if (directionAtEndOfDodge != Vector2.zero)
            SetMoveInput(directionAtEndOfDodge);
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
        public UnityEngine.UI.Slider ChargeSlider;
        public GameObject ChargeIndicators;
        public enum EChargeType
        {
            Slider,
            States
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

        internal bool charging;
        internal float latestCharge;
        internal UnityAction attackLaunched;

        public enum AttackState
        {
            Ready,
            Attacking,
            OnCooldown
        }
        public AttackState attackState;

        internal void Launch(int chargeIndex)
        {
            WeaponAnimator.SetTrigger("Attack");
            WeaponAnimator.SetInteger("AttackIndex", chargeIndex);
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
            attackState = AttackState.Attacking;
        }

        GameObject GetChargeObject()
        {
            GameObject obj;
            switch (ChargeType)
            {
                default:
                case EChargeType.Slider:
                    obj = ChargeSlider.gameObject;
                    break;
                case EChargeType.States:
                    obj = ChargeIndicators.gameObject;
                    break;
            }
            if (obj == null)
            {
                Debug.LogWarning("Charge object " + ChargeType + " of Player is null!");
            }
            return obj;
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
            if (!charging)
            {
                Debug.Log("No charge to interrupt");
                return;
            }
            charging = false;
        }

        public IEnumerator StartCharge()
        {
            charging = true;
            switch (ChargeType)
            {
                case EChargeType.Slider:
                    ChargeIndicator.SetCurrent(0, false);
                    break;
                case EChargeType.States:
                    ChargeIndicator.SetCurrent(0);
                    break;
            }
            float chargeTime = 0f;
            float chargeTimeClamped = 0f;
            Debug.Log("Start charge");
            while (chargeTime < ChargeTimeDeadzone)
            {
                yield return new WaitForEndOfFrame();
                chargeTime += Time.unscaledDeltaTime;
            }
            Debug.Log("Charge deadzone passed");
            GetChargeObject().SetActive(true);
            bool slowmotionInitiated = false;
            while (charging)
            {
                yield return new WaitForEndOfFrame();
                chargeTime += Time.unscaledDeltaTime;
                //if (ChargeEffectedBySlowdown)
                //{
                //    chargeTime = Time.time - chargeStart;
                //}
                //else
                //{
                //    chargeTime = Time.unscaledTime - chargeStart;
                //}
                
                chargeTimeClamped = Mathf.Clamp01(chargeTime / ChargeTime);
                //Debug.Log("Chargetime clamped: " + chargeTimeClamped);
                switch (ChargeType)
                {
                    case EChargeType.Slider:
                        ChargeSlider.value = chargeTimeClamped;
                        ChargeIndicator.SetCurrent(GetChargeZoneIndex(chargeTimeClamped) +1, false);
                        break;
                    case EChargeType.States:
                        ChargeIndicator.SetCurrent(GetChargeZoneIndex(chargeTimeClamped) +1);
                        break;
                }
                if (!slowmotionInitiated && chargeTimeClamped > .5f)
                {
                    slowmotionInitiated = true;
                    TimeManager.Instance.DoSlowmotion(.25f);
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

    public void AttemptAttack()
    {
        if (!Stamina.Use())
        {
            Debug.Log("Not enough stamina to attack");
            return;
        }
        StartCoroutine(attacking.StartCharge());
    }

    public void OnAttack()
    {
        Animator.SetFloat("AttackCharge", attacking.latestCharge);
        Animator.SetTrigger("Attack");
        attacking.WeaponAnimator.SetFloat("AttackCharge", attacking.latestCharge);
        attacking.WeaponAnimator.SetTrigger("Attack");

        //Animator.ResetTrigger("Attack");
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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = targeting.RadiusColor;
        Handles.DrawWireDisc(transform.position, transform.up, targeting.Radius);
    }
#endif
}
