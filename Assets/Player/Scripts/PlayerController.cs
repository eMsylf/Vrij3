using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Combat;
using UnityEditor;

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
    public bool SpawnPosSet = false;

    private void OnEnable()
    {
        Debug.Log("Player enabled");
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
        Debug.Log("Player disabled");
        Controls.Game.Disable();
        UnsubControls();

        attacking.attackLaunched -= () => OnAttack();
    }

    bool controlsSubscribed;
    private void SubscribeControls()
    {
        if (controlsSubscribed)
            return;
        Controls.Game.Movement.performed += _ => Move(_.ReadValue<Vector2>());
        Controls.Game.Movement.canceled += _ => Stop();
        Controls.Game.Dodge.performed += _ => Dodge();
        Controls.Game.Attack.performed += _ => StartCoroutine(attacking.StartCharge());
        Controls.Game.Attack.canceled += _ => attacking.charging = false;
        Controls.Game.LockOn.performed += _ => targeting.LockOn(transform.position);
        controlsSubscribed = true;
    }

    private void UnsubControls()
    {
        if (!controlsSubscribed)
            return;
        Controls.Game.Movement.performed -= _ => Move(_.ReadValue<Vector2>());
        Controls.Game.Movement.canceled -= _ => Stop();
        Controls.Game.Dodge.performed -= _ => Dodge();
        Controls.Game.Attack.performed -= _ => StartCoroutine(attacking.StartCharge());
        Controls.Game.Attack.canceled -= _ => attacking.charging = false;
        Controls.Game.LockOn.performed -= _ => targeting.LockOn(transform.position);
        controlsSubscribed = false;
    }

    private void FixedUpdate()
    {
        switch (State)
        {
            case EState.Idle:
            case EState.Moving:
                break;
            case EState.Dodging:
                movement.CalculatedMovement = movement.DodgeDirection * movement.DodgeSpeed;
                break;
            case EState.Stunned:
                break;
        }
        Rigidbody.MovePosition(Rigidbody.position + movement.GetTopDownMovement() * 50f * Time.fixedDeltaTime);
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
        public bool ApplyMovementInput = true;
        internal Vector2 Input;
        internal Vector2 FacingDirection = new Vector2(0f, 1f);
        internal Vector2 CalculatedMovement;
        internal Vector3 GetTopDownMovement()
        {
            return new Vector3(CalculatedMovement.x, 0f, CalculatedMovement.y);
        }
        internal Vector2 DodgeDirection;
    }
    public Movement movement;

    private void Move(Vector2 input)
    {
        if (!movement.ApplyMovementInput)
        {
            return;
        }
        movement.Input = input;
        switch (State)
        {
            case EState.Idle:
            case EState.Moving:
                movement.CalculatedMovement = movement.Input * movement.Speed;
                break;
            case EState.Stunned:
                movement.CalculatedMovement = movement.Input * 0f;
                break;
        }
        Animator.SetBool("IsWalking", true);

        UpdateFacingDirection(movement.Input);
    }

    private void Stop()
    {
        if (!movement.ApplyMovementInput)
            return;
        //Debug.Log("Stop!");
        movement.Input = Vector2.zero;
        movement.CalculatedMovement = movement.Input;
        //UpdateAnimatorDirection(Direction.UpdateLookDirection(MovementInput));
        State = EState.Idle;
        Animator.SetBool("IsWalking", false);
    }

    private void Dodge()
    {
        if (State == EState.Dodging)
        {
            return;
        }

        StartCoroutine(Dodge(movement.DodgeDuration));
    }

    private IEnumerator Dodge(float duration)
    {
        State = EState.Dodging;
        movement.ApplyMovementInput = false;
        if (movement.Input == Vector2Int.zero)
        {
            movement.DodgeDirection = movement.FacingDirection * -1f;
        }
        else
        {
            movement.DodgeDirection = movement.Input;
        }

        yield return new WaitForSeconds(duration);

        State = EState.Idle;
        Vector2 directionAtEndOfDodge = Controls.Game.Movement.ReadValue<Vector2>();
        movement.ApplyMovementInput = true;
        if (directionAtEndOfDodge != Vector2.zero)
            Move(directionAtEndOfDodge);
        else
        {
            Stop();
        }
    }

    public void UpdateFacingDirection(Vector2 direction)
    {
        movement.FacingDirection = direction;
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
        public Statistic Charge;
        [Tooltip("Time it takes for the slider to fill up")]
        public float ChargeTime = 2f;
        [Tooltip("Time below which a charge will not be initiated")]
        public float ChargeTimeDeadzone = .1f;
        public bool ChargeEffectedBySlowdown = false;

        //public UnityEvent OnAttackEnd;

        internal bool charging;
        internal float latestCharge;
        internal UnityAction attackLaunched;

        public enum AttackState
        {
            Ready,
            CurrentlyAttacking,
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
            attackState = AttackState.CurrentlyAttacking;
        }

        GameObject GetChargeObject()
        {
            switch (ChargeType)
            {
                default:
                case EChargeType.Slider:
                    return ChargeSlider.gameObject;
                case EChargeType.States:
                    return ChargeIndicators.gameObject;
            }
        }

        int GetChargeZoneIndex(float time)
        {
            for (int i = 0; i < ChargeZones.colorKeys.Length; i++)
            {
                if (ChargeZones.colorKeys[i].time > time)
                {
                    Debug.Log("Time: " + time + " index " + i);
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
            float chargeStart = Time.time;
            float chargeTime = 0f;
            float chargeTimeClamped = 0f;
            Debug.Log("Start charge");
            while (chargeTime < ChargeTimeDeadzone)
            {
                yield return new WaitForEndOfFrame();
                chargeTime = Time.time - chargeStart;
            }
            Debug.Log("Charge deadzone passed");
            GetChargeObject().SetActive(true);
            bool slowmotionInitiated = false;
            while (charging)
            {
                yield return new WaitForEndOfFrame();
                chargeTime += Time.deltaTime / Time.timeScale;
                //if (ChargeEffectedBySlowdown)
                //{
                //    chargeTime = Time.time - chargeStart;
                //}
                //else
                //{
                //    chargeTime = Time.unscaledTime - chargeStart;
                //}
                
                chargeTimeClamped = Mathf.Clamp01(chargeTime / ChargeTime);
                Debug.Log("Chargetime clamped: " + chargeTimeClamped);
                switch (ChargeType)
                {
                    case EChargeType.Slider:
                        ChargeSlider.value = chargeTimeClamped;
                        break;
                    case EChargeType.States:
                        Charge.SetCurrent(GetChargeZoneIndex(chargeTimeClamped) +1);
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

            Launch(chargeTimeClamped);
        }
    }
    public Attacking attacking;

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
