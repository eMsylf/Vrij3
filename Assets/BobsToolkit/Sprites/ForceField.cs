using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TriggerList))]
public class ForceField : MonoBehaviour
{
    public float Force = 1f;

    private TriggerList triggerList;
    public TriggerList GetTriggerList()
    {
        if (triggerList == null)
            triggerList = GetComponent<TriggerList>();
        return triggerList;
    }

    private void FixedUpdate()
    {
        foreach (GameObject obj in GetTriggerList().gameObjects)
        {
            Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                //Debug.Log("Could not find rigidbody on " + obj.name, obj);
                rigidbody = obj.GetComponentInParent<Rigidbody>();
            }

            if (rigidbody == null)
            {
                //Debug.Log("Could not find rigidbody on parent" + obj.name, obj);
                continue;
            }

            if (rigidbody.isKinematic)
            {
                //Debug.Log(rigidbody.name + " is kinematic", rigidbody);
                continue;
            }

            float Distance = Vector3.Distance(rigidbody.position, transform.position);
            if (Distance == 0f)
            {
                //Debug.Log("Distance from " + name + " to " + obj.name + " is zero", this);
                continue;
            }

            Vector3 direction = rigidbody.position - transform.position;

            Vector3 calculatedForce = direction.normalized * (Force);

            //Debug.Log("Applying " + calculatedForce + " force to: " + rigidbody.name + " in " + name, this);

            rigidbody.AddForce(calculatedForce, ForceMode.Acceleration);
        }
    }
}
