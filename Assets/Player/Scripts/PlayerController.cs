using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public enum EState
    {
        Idle = default,
        Moving,
        Dodging,
        Attacking,
        Hit
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

        attacking.attackLaunched += () => OnAttack();
    }

    private void FixedUpdate()
    {
        switch (movement._MoveState)
        {
            case Movement.MoveState.Idle:
            case Movement.MoveState.Moving:
                break;
            case Movement.MoveState.Dodging:
                movement.CalculatedMovement = movement.DodgeDirection * movement.DodgeSpeed;
                break;
            case Movement.MoveState.Stunned:
                break;
        }
        Rigidbody.MovePosition(Rigidbody.position + movement.GetTopDownMovement());
    }

    private void Move(Vector2 input)
    {
        if (!movement.ApplyMovementInput)
        {
            return;
        }
        movement.Input = input;
        switch (movement._MoveState)
        {
            case Movement.MoveState.Idle:
            case Movement.MoveState.Moving:
                movement.CalculatedMovement = movement.Input * movement.Speed;
                break;
            case Movement.MoveState.Stunned:
                movement.CalculatedMovement = movement.Input * 0f;
                break;
        }
        UpdateAnimatorDirection();
    }

    private void Stop()
    {
        if (!movement.ApplyMovementInput)
            return;
        //Debug.Log("Stop!");
        movement.Input = Vector2.zero;
        movement.CalculatedMovement = movement.Input;
        //UpdateAnimatorDirection(Direction.UpdateLookDirection(MovementInput));
        movement._MoveState = Movement.MoveState.Idle;
        State = EState.Idle;
    }

    private void Dodge()
    {
        if (movement._MoveState == Movement.MoveState.Dodging)
        {
            return;
        }

        StartCoroutine(Dodge(movement.DodgeDuration));
    }

    private IEnumerator Dodge(float duration)
    {
        movement._MoveState = Movement.MoveState.Dodging;
        State = EState.Dodging;
        movement.ApplyMovementInput = false;
        movement.DodgeDirection = movement.Input;
        
        yield return new WaitForSeconds(duration);

        movement._MoveState = Movement.MoveState.Moving;
        State = EState.Moving;
        Vector2 directionAtEndOfDodge = Controls.Game.Movement.ReadValue<Vector2>();
        movement.ApplyMovementInput = true;
        Move(directionAtEndOfDodge);
    }

    public void UpdateAnimatorDirection()
    {
        if (Animator == null)
            return;
        Animator.SetFloat("Hor", movement.Input.x);
        Animator.SetFloat("Vert", movement.Input.y);
    }

    public void OnAttack()
    {
        Debug.Log("Attack");
        Animator.SetFloat("AttackCharge", attacking.latestCharge);
        Animator.SetTrigger("Attack");
        //Animator.ResetTrigger("Attack");
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


        internal MoveState _MoveState;
        internal enum MoveState
        {
            Moving,
            Dodging,
            Idle,
            Stunned
        }
    }
    public Movement movement;

    [System.Serializable]
    public class Attacking
    {
        public UnityEngine.UI.Slider ChargeSlider;
        //public Gradient ChargeZones;
        [Tooltip("Time it takes for the slider to fill up")]
        public float ChargeTime = 2f;
        [Tooltip("Time below which a charge will not be initiated")]
        public float ChargeTimeDeadzone = .1f;
        internal bool charging;
        internal float latestCharge;
        internal event UnityAction attackLaunched;
        //public enum ColorChannel
        //{
        //    r = default,
        //    g,
        //    b,
        //    a
        //}
        //public ColorChannel EvaluatedColorChannel;

        //internal Color GetChargeZone(float time)
        //{
        //    return ChargeZones.Evaluate(time);
        //}

        //internal float GetChargeZone(float time, ColorChannel channel)
        //{
        //    switch (channel)
        //    {
        //        case ColorChannel.r:
        //            return ChargeZones.Evaluate(time).r;
        //        case ColorChannel.g:
        //            return ChargeZones.Evaluate(time).g;
        //        case ColorChannel.b:
        //            return ChargeZones.Evaluate(time).b;
        //        case ColorChannel.a:
        //            return ChargeZones.Evaluate(time).a;
        //        default:
        //            return ChargeZones.Evaluate(time).r;
        //    }
        //}

        internal void Launch(float chargeTime)
        {
            if (chargeTime == 0f)
            {
                Debug.Log("Launch uncharged attack!");
                return;
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
            
            while (charging)
            {
                yield return new WaitForEndOfFrame();
                chargeTime = Time.time - chargeStart;
                chargeTimeClamped = chargeTime / ChargeTime;
                ChargeSlider.value = chargeTimeClamped;
            }
            ChargeSlider.gameObject.SetActive(false);

            Launch(chargeTimeClamped);
        }
    }
    public Attacking attacking;

    [System.Serializable]
    public class Targeting
    {
        public GameObject DebugTarget;
    }
    public Targeting targeting;
}
