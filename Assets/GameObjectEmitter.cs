using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameObjectEmitter : MonoBehaviour
{
    [Tooltip("To make the emission speed work, make sure the object has a rigidbody assigned")]
    public GameObject prefab;

    [Min(0)]
    public int numberOfObjects = 5;
    public float EmissionSpeed = 1f;

    [Min(0)]
    public float Interval = 1f;
    public bool makeEmittedChildren = false;

    [Tooltip("Objects are destroyed after this many seconds")]
    public float objectLifetime = 1f;

    private float interval;
    private void Update()
    {
        if (interval > 0f)
        {
            interval -= Time.deltaTime;
            return;
        }

        interval = Interval;

        Emit();
    }

    public void Emit()
    {
        Debug.Log("Emit");

        Vector3[] directions = GetDirections();

        for (int i = 0; i < directions.Length; i++)
        {
            GameObject emittedObject;
            //Vector3 emissionPos;
            if (makeEmittedChildren)
            {
                emittedObject = Instantiate(prefab, transform);
            }
            else
            {
                emittedObject = Instantiate(prefab);
                emittedObject.transform.position = transform.position;
            }

            Destroy(emittedObject, objectLifetime);
            Debug.Log("Hallo");

            Vector3 direction = directions[i];
            Rigidbody objRigidbody = emittedObject.GetComponent<Rigidbody>();
            if (objRigidbody != null)
            {
                //direction = Vector3.Scale(direction, transform.forward);
                //direction = Vector3.Scale(direction, transform.right);
                direction = PlayerController.ConvertToObjectRelative(transform, direction, false);

                objRigidbody.AddForce(direction, ForceMode.VelocityChange);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Handles.matrix = transform.localToWorldMatrix;
        Vector3[] directions = GetDirections();
        for (int i = 0; i < directions.Length; i++)
        {
            //Debug.Log("Direction: " + directions[i]);
            Handles.DrawLine(Vector3.zero, directions[i] * EmissionSpeed);
        }
    }

    public Vector3[] GetDirections()
    {
        Vector3[] directions = new Vector3[numberOfObjects];

        for (int i = 0; i < numberOfObjects; i++)
        {
            float prog = (i / (float)numberOfObjects) * Mathf.PI * 2;
            float xOffset = Mathf.Sin(prog);
            float zOffzet = Mathf.Cos(prog);
            Vector3 direction = new Vector3(xOffset, 0f, zOffzet);
            directions[i] = direction;
            //Debug.Log("i: " + i + " Progress: " + prog + " Direction: " + direction);
        }

        return directions;
    }
}
