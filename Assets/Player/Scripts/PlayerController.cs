using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
        Controls.Game.Attack.performed += _ => StartCoroutine(attack.Charge());
        Controls.Game.Attack.canceled += _ => attack.charging = false;
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
        movement.Input = input;
        //Debug.Log("Move! " + input);
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
        //Debug.Log("Stop!");
        movement.Input = Vector2.zero;
        movement.CalculatedMovement = movement.Input;
        //UpdateAnimatorDirection(Direction.UpdateLookDirection(MovementInput));
        movement._MoveState = Movement.MoveState.Idle;
    }

    private void Dodge()
    {
        if (movement._MoveState == Movement.MoveState.Dodging)
        {
            Debug.Log("Already dodging");
            return;
        }

        StartCoroutine(Dodge(movement.DodgeDuration));
    }

    private IEnumerator Dodge(float duration)
    {
        movement._MoveState = Movement.MoveState.Dodging;
        movement.DodgeDirection = movement.Input;
        Debug.Log("Dodge!");
        
        yield return new WaitForSeconds(duration);

        Debug.Log("Dodge end");
        movement._MoveState = Movement.MoveState.Moving;
        Vector2 directionAtEndOfDodge = Controls.Game.Movement.ReadValue<Vector2>();
        Debug.Log("Held direction at the end of dash: " + directionAtEndOfDodge);
        Move(directionAtEndOfDodge);
    }

    

    public void UpdateAnimatorDirection()
    {
        if (Animator == null)
            return;
        //Debug.Log("Set animator thingies", Animator);
        Animator.SetFloat("Hor", movement.Input.x);
        Animator.SetFloat("Vert", movement.Input.y);
    }

    [System.Serializable]
    public class Movement
    {
        public float Speed = 1f;
        public float DodgeSpeed = 2f;
        public float DodgeDuration = 1f;
        internal Vector2 Input;
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
    public class Attack
    {
        public UnityEngine.UI.Slider ChargeSlider;
        public Gradient ChargeZones;
        [Tooltip("Charge speed of the slider in sliders per second")]
        [Range(0f, 1f)]
        public float ChargeSpeed = .25f;
        internal bool charging;

        internal Color GetAttackZone(float time)
        {
            return ChargeZones.Evaluate(time);
        }

        void Launch(float chargeTime)
        {
            if (chargeTime == 0f)
            {
                Debug.Log("Launch uncharged attack!");
                return;
            }

            Debug.Log("Launch charged attack. Charge amount: " + GetAttackZone(chargeTime));
            //if (attack.ChargeSlider == null)
            //{
            //    Debug.LogError("Attack charge slider is null!", this);
            //    return;
            //}
        }

        //void StartCharge()
        //{
        //    Debug.Log("Start attack charge!");
        //}

        public IEnumerator Charge()
        {
            charging = true;
            float chargeStart = Time.time;
            float chargeTime = 0f;
            Debug.Log("Start charge");
            while (charging)
            {
                yield return new WaitForEndOfFrame();
                float delta = Time.time - chargeStart;
                chargeTime += delta;
                ChargeSlider.value = chargeTime;
                chargeTime = Mathf.Clamp01(chargeTime);
            }
            Launch(chargeTime);
        }

        // Attempt at creating a standard attack zone setting (does not get called)
        //Attack()
        //{
        //    ChargeZones.colorKeys = new GradientColorKey[4];
        //    ChargeZones.colorKeys[0].color = Color.green;
        //    ChargeZones.colorKeys[0].time = 0f;
        //    ChargeZones.colorKeys[1].color = Color.blue;
        //    ChargeZones.colorKeys[1].time = .33f;
        //    ChargeZones.colorKeys[2].color = Color.yellow;
        //    ChargeZones.colorKeys[2].time = .66f;
        //    ChargeZones.colorKeys[3].color = Color.red;
        //    ChargeZones.colorKeys[3].time = 1f;
        //}
    }
    public Attack attack;

    [System.Serializable]
    public class Targeting
    {
        public GameObject DebugTarget;
    }
    public Targeting targeting;
}
