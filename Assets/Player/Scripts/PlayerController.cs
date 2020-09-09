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
                animator = GetComponentInChildren<Animator>();
            }
            return animator;
        }
    }

    public Direction Direction;

    Vector2 MovementInput;
    Vector2 Movement;
    public float Speed = 1f;
    public float DodgeSpeed = 2f;
    public float DodgeDuration = 1f;
    public Vector2 DodgeDirection;

    private MoveState _MoveState;
    private enum MoveState
    {
        Moving,
        Dodging,
        Idle,
        Stunned
    }

    private void Awake()
    {
        Controls.Game.Enable();
        Controls.Game.Movement.performed += _ => Move(_.ReadValue<Vector2>());
        Controls.Game.Movement.canceled += _ => Stop();
        Controls.Game.Dodge.performed += _ => Dodge();
    }

    private void FixedUpdate()
    {
        switch (_MoveState)
        {
            case MoveState.Idle:
            case MoveState.Moving:
                break;
            case MoveState.Dodging:
                Movement = DodgeDirection * DodgeSpeed;
                break;
            case MoveState.Stunned:
                break;
        }
        Rigidbody.MovePosition(Rigidbody.position + new Vector3(Movement.x, 0f, Movement.y));
    }

    private void Move(Vector2 input)
    {
        MovementInput = input;
        Direction.MovementIndicator.localPosition = new Vector3(MovementInput.x, 0f, MovementInput.y);
        //Debug.Log("Move! " + input);
        switch (_MoveState)
        {
            case MoveState.Idle:
            case MoveState.Moving:
                Movement = MovementInput * Speed;
                break;
            case MoveState.Stunned:
                Movement = MovementInput * 0f;
                break;
        }
        UpdateAnimatorDirection(Direction.UpdateLookDirection(MovementInput));
    }

    private void Stop()
    {
        //Debug.Log("Stop!");
        MovementInput = Vector2.zero;
        Movement = MovementInput;
        //UpdateAnimatorDirection(Direction.UpdateLookDirection(MovementInput));
        _MoveState = MoveState.Idle;
    }

    private void Dodge()
    {
        if (_MoveState == MoveState.Dodging)
        {
            Debug.Log("Already dodging");
            return;
        }

        StartCoroutine(Dodge(DodgeDuration));
    }

    private IEnumerator Dodge(float duration)
    {
        _MoveState = MoveState.Dodging;
        DodgeDirection = MovementInput;
        Debug.Log("Dodge!");
        
        yield return new WaitForSeconds(duration);

        Debug.Log("Dodge end");
        _MoveState = MoveState.Moving;
        Vector2 directionAtEndOfDodge = Controls.Game.Movement.ReadValue<Vector2>();
        Debug.Log("Held direction at the end of dash: " + directionAtEndOfDodge);
        Move(directionAtEndOfDodge);
    }

    public void UpdateAnimatorDirection(ELookDirection direction)
    {
        if (Animator == null)
            return;

        Animator.SetInteger("Direction", (int)direction);
    }

    //private bool InputLock = false;
    //private void LockInput()
    //{
    //    Debug.Log("Lock input");
    //    InputLock = true;
    //}

    //private void UnlockInput()
    //{
    //    Debug.Log("Unlock input");
    //    InputLock = false;
    //}
}
