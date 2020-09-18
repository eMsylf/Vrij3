using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Jumpy : MonoBehaviour
{
    public float JumpPower = 5f;
    public Vector3 Torque = Vector3.zero;
    public float Interval = 1f;

    new Rigidbody rigidbody;
    Rigidbody Rigidbody
    {
        get
        {
            if (rigidbody == null)
                rigidbody = GetComponent<Rigidbody>();
            return rigidbody;
        }
    }

    void Update()
    {
        if (Rigidbody.velocity == Vector3.zero)
        {
            Jump(JumpPower);
        }
    }

    void Jump(float power)
    {
        //Debug.Log("Jump");
        Rigidbody.AddForce(Vector3.up * power, ForceMode.Impulse);
        Rigidbody.AddTorque(Torque, ForceMode.Impulse);
    }
}
