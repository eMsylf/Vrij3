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

    Vector2 MovementInput;
    public float Speed = 1f;
    public float DodgeSpeed = 2f;
    public float DodgeDuration = 1f;
    public Vector2 DodgeDirection;

    public MoveState _MoveState;
    public enum MoveState
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
            case MoveState.Moving:
                break;
            case MoveState.Dodging:
                MovementInput = DodgeDirection * DodgeSpeed;
                break;
            case MoveState.Stunned:
                break;
        }
        Rigidbody.MovePosition(Rigidbody.position + new Vector3(MovementInput.x, 0f, MovementInput.y));
    }

    private void Move(Vector2 input)
    {
        //Debug.Log("Move! " + input);
        switch (_MoveState)
        {
            case MoveState.Moving:
                MovementInput = input * Speed;
                break;
            case MoveState.Stunned:
                MovementInput = input * 0f;
                break;
        }
    }


    private void Stop()
    {
        //Debug.Log("Stop!");
        MovementInput = Vector2.zero;
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
        Stop();
        _MoveState = MoveState.Moving;
    }

    private bool InputLock = false;
    private void LockInput()
    {
        Debug.Log("Lock input");
        InputLock = true;
    }

    private void UnlockInput()
    {
        Debug.Log("Unlock input");
        InputLock = false;
    }
}
