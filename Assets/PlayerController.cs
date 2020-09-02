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

    private void Awake()
    {
        Controls.InGame.Enable();
        Controls.InGame.Movement.performed += _ => Move(_.ReadValue<Vector2>());
        Debug.Log("Hi");
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        Rigidbody.AddForce(MovementInput.x, 0f, MovementInput.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Move(Vector2 input)
    {
        Debug.Log("Move! " + input);
        MovementInput = input * Speed;
    }
}
