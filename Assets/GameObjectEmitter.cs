using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using BobJeltes;
using BobJeltes.Extensions;

public class GameObjectEmitter : MonoBehaviour
{
    [Tooltip("To make the emission speed work, make sure the object has a rigidbody assigned")]
    public GameObject prefab;
    
    [Space]
    public bool play = true;
    [Tooltip("'Play' will be set to true upon awakening")]
    public bool playOnAwake = false;
    [Tooltip("An emission will fire upon awakening")]
    public bool emitOnAwake = false;
    [Space]
    [Min(0)]
    public int numberOfObjects = 5;
    [Range(0f, 360f)]
    public float emissionAngle = 360f;
    [Tooltip("If ticked, the directions will be exactly distributed over the entire angle. " +
        "\nIf unticked, the directions will be distributed in a way that is most useful if the distribution is (close to) full-circle, when emission angle = 360." +
        "\n This is so the first and last direction don't overlap.")]
    public bool exactAngle = false;
    public float emissionSpeed = 1f;
    [Space]
    [Min(0)]
    public float Interval = 1f;
    private float interval;
    [Space]
    [Tooltip("Objects are destroyed after this many seconds.")]
    [Min(0)]
    public float objectLifetime = 1f;
    public bool objectsBecomeChildren = false;

    public bool useObjectPool = true;
    public ObjectPool objectPool;
    public ObjectPool GetObjectPool()
    {
        if (objectPool == null)
        {
            objectPool = gameObject.GetComponent<ObjectPool>();
            if (objectPool == null)
            {
                objectPool = gameObject.AddComponent<ObjectPool>();
            }
        }
        if (objectPool.prefab == null)
            objectPool.prefab = prefab;
        return objectPool;
    }

    private void Awake()
    {
        if (playOnAwake)
            play = true;
        if (emitOnAwake)
            Emit();
    }

    private void FixedUpdate()
    {
        if (!play)
            return;

        if (interval > 0f)
        {
            interval -= Time.fixedDeltaTime;
            return;
        }

        interval = Interval;

        Emit();
    }

    public void Emit()
    {
        //Debug.Log("Emit");

        Vector3[] directions = GetDirections();
        ObjectPool objPool = GetObjectPool();
        for (int i = 0; i < directions.Length; i++)
        {
            GameObject emittedObject;

            if (useObjectPool)
            {
                emittedObject = objPool.GetInactive();
                emittedObject.SetActive(true);
                // Deactivate object after its lifetime has ended
                StartCoroutine(emittedObject.SetActive(false, objectLifetime));
            }
            else
            {
                emittedObject = Instantiate(prefab, transform);
                Destroy(emittedObject, objectLifetime);
            }

            emittedObject.transform.position = transform.position;
            if (objectsBecomeChildren)
            {
                emittedObject.transform.SetParent(transform);
            }

            Vector3 direction = directions[i];
            Rigidbody objRigidbody = emittedObject.GetComponent<Rigidbody>();
            if (objRigidbody != null)
            {
                objRigidbody.velocity = Vector3.zero;
                objRigidbody.AddForce(direction.ConvertToObjectRelative(transform, false) * emissionSpeed, ForceMode.VelocityChange);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Handles.matrix = transform.localToWorldMatrix;
        Vector3[] directions = GetDirections();
        for (int i = 0; i < directions.Length; i++)
        {
            Vector3 objEnd = directions[i] * emissionSpeed;
            if (objectLifetime > 0f)
            {
                objEnd *= objectLifetime; // Hier was ik
            }
            //Debug.Log("Direction: " + directions[i]);
            Handles.DrawLine(Vector3.zero, objEnd);
            Handles.ArrowHandleCap(0, objEnd, Quaternion.LookRotation(directions[i]), 1f, EventType.Repaint);
        }
        if (numberOfObjects > 0f && emissionAngle > 0f)
        {
            Handles.DrawWireArc(Vector3.zero, Vector3.up, directions[0], emissionAngle, 1f);
        }
    }

    public Vector3[] GetDirections()
    {
        Vector3[] directions = new Vector3[numberOfObjects];

        float whole = numberOfObjects;
        if (exactAngle)
        {
            whole -= 1f;
        }

        for (int i = 0; i < numberOfObjects; i++)
        {
            float prog = (i / whole) * Mathf.PI * (emissionAngle / 360f) * 2f;
            float xOffset = Mathf.Sin(prog);
            float zOffzet = Mathf.Cos(prog);
            Vector3 direction = new Vector3(xOffset, 0f, zOffzet);
            directions[i] = direction;
            //Debug.Log("i: " + i + " Progress: " + prog + " Direction: " + direction);
        }

        return directions;
    }
}
