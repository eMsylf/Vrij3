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

    private void Awake()
    {
        Controls.Game.Enable();
        Controls.Game.Movement.performed += _ => Move(_.ReadValue<Vector2>());
        Controls.Game.Movement.canceled += _ => Stop();
        Controls.Game.Dodge.performed += _ => Dodge();
        Controls.Game.Attack.performed += _ => StartCoroutine(attacking.Charge());
        Controls.Game.Attack.canceled += _ => attacking.charging = false;
        Controls.Game.LockOn.performed += _ => targeting.LockOn(transform.position);

        attacking.attackLaunched += () => OnAttack();
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == Rigidbody)
        {
            Debug.Log("Hit self");
            return;
        }
    }

    public void OnAttack()
    {
        //Debug.Log("Attack");
        Animator.SetFloat("AttackCharge", attacking.latestCharge);
        Animator.SetTrigger("Attack");
        attacking.WeaponAnimator.SetFloat("AttackCharge", attacking.latestCharge);
        attacking.WeaponAnimator.SetTrigger("Attack");

        //Animator.ResetTrigger("Attack");
    }

    public Direction Direction;
    public void UpdateDirectionIndicator()
    {
        Direction.DirectionIndicator.localPosition = new Vector3(movement.FacingDirection.x, 0f, movement.FacingDirection.y);
    }


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

    [System.Serializable]
    public class Attacking
    {
        public UnityEngine.UI.Slider ChargeSlider;
        public Animator WeaponAnimator;
        //public Gradient ChargeZones;
        [Tooltip("Time it takes for the slider to fill up")]
        public float ChargeTime = 2f;
        [Tooltip("Time below which a charge will not be initiated")]
        public float ChargeTimeDeadzone = .1f;

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


        public void InterruptCharge()
        {
            charging = false;
        }

        public IEnumerator Charge()
        {
            charging = true;
            float chargeStart = Time.time;
            float chargeTime = 0f;
            float chargeTimeClamped = 0f;
            //Debug.Log("Start charge");
            while (chargeTime < ChargeTimeDeadzone)
            {
                yield return new WaitForEndOfFrame();
                chargeTime = Time.time - chargeStart;
            }
            
            ChargeSlider.gameObject.SetActive(true);
            bool slowmotionInitiated = false;
            while (charging)
            {
                yield return new WaitForEndOfFrame();
                chargeTime = Time.time - chargeStart;
                chargeTimeClamped = Mathf.Clamp01(chargeTime / ChargeTime);
                ChargeSlider.value = chargeTimeClamped;
                if (!slowmotionInitiated && chargeTimeClamped > .5f)
                {
                    slowmotionInitiated = true;
                    TimeManager.Instance.DoSlowmotion(.25f);
                }
            }
            TimeManager.Instance.StopSlowmotion();
            ChargeSlider.gameObject.SetActive(false);

            Launch(chargeTimeClamped);
        }
    }
    public Attacking attacking;

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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = targeting.RadiusColor;
        Handles.DrawWireDisc(transform.position, transform.up, targeting.Radius);
    }
#endif
}
