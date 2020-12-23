using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using BobJeltes;
using BobJeltes.Extensions;
using UnityEngine.Rendering;

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
    //[Min(0)]
    //public float startDistance;
    [Min(0)]
    public Vector3 startWidth = Vector3.one;
    [Range(-90f, 90f)]
    public float verticalAngle = 0f;
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
    public bool useLifetime = true;
    [Tooltip("Objects are destroyed after this many seconds.")]
    [Min(0)]
    public float objectLifetime = 1f;
    public bool objectsBecomeChildren = false;
    public bool syncDeactivation = true;

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

    private void OnDisable()
    {
        if (syncDeactivation)
        {
            ObjectPool objPool = GetObjectPool();
            foreach (GameObject obj in objPool.objectPool)
            {
                if (obj == null)
                    continue;
                obj.SetActive(false);
            }
        }
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

    public virtual void Emit()
    {
        //Debug.Log("Emit");
        Vector3[] starts = GetStartPositions();
        Vector3[] directions = GetDirections();
        ObjectPool objPool = GetObjectPool();
        for (int i = 0; i < directions.Length; i++)
        {
            GameObject emittedObject;
            Vector3 direction = directions[i];

            if (useObjectPool)
            {
                emittedObject = objPool.GetInactive();
                emittedObject.SetActive(true);
                // Deactivate object after its lifetime has ended
                //StopCoroutine(emittedObject.SetActive(false, objectLifetime));
                if (useLifetime)
                {
                    DeactivateAfter deactivate = emittedObject.GetComponent<DeactivateAfter>();
                    if (deactivate == null)
                        Debug.LogError("Deactivate After component is missing on the prefab!", prefab);
                    else
                    {
                        deactivate.enabled = true;
                        deactivate.Restart(objectLifetime);
                    }
                    //StartCoroutine(emittedObject.SetActive(false, objectLifetime));
                }
            }
            else
            {
                emittedObject = Instantiate(prefab, transform);
                Destroy(emittedObject, objectLifetime);
            }

            Vector3 objStart = starts[i];

            emittedObject.transform.position = transform.position + objStart.ConvertToObjectRelative(transform);
            if (objectsBecomeChildren)
            {
                emittedObject.transform.SetParent(transform);
            }

            Rigidbody objRigidbody = emittedObject.GetComponent<Rigidbody>();
            if (objRigidbody != null)
            {
                objRigidbody.angularVelocity = Vector3.zero;
                objRigidbody.velocity = Vector3.zero;
                objRigidbody.AddForce(direction.ConvertToObjectRelative(transform, false) * emissionSpeed, ForceMode.VelocityChange);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.matrix = transform.localToWorldMatrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3[] starts = GetStartPositions();
        Vector3[] directions = GetDirections();
        for (int i = 0; i < directions.Length; i++)
        {
            Vector3 objStart = starts[i];
            Vector3 objEnd = directions[i];
            if (!Mathf.Approximately(emissionSpeed, 0f)) objEnd *= emissionSpeed;
            if (useLifetime && objectLifetime > 0f) objEnd *= objectLifetime;
            objEnd += objStart;
            //Debug.Log("Direction: " + directions[i]);
            Handles.DrawLine(objStart, objEnd);
            Handles.ArrowHandleCap(0, objEnd, Quaternion.LookRotation(objEnd - objStart), 1f, EventType.Repaint);
        }
        //if (numberOfObjects > 0f && emissionAngle > 0f)
        //{
        //    //Handles.DrawWireArc(Vector3.zero, Vector3.up, Vector3.Scale(directions[0], startWidth), emissionAngle, Mathf.Sqrt(startDistance) * startWidth.magnitude);
        //    //Gizmos.DrawWireSphere(Vector3.zero, startWidth.magnitude);

        //}
    }
#endif

    public Vector3[] GetStartPositions()
    {
        Vector3[] starts = new Vector3[numberOfObjects];

        float whole = numberOfObjects;
        if (exactAngle)
        {
            whole -= 1f;
        }

        for (int i = 0; i < numberOfObjects; i++)
        {
            float prog = (i / whole) * Mathf.PI * (emissionAngle / 360f) * 2f;
            
            float xOffset = Mathf.Sin(prog) * startWidth.x;
            float zOffzet = Mathf.Cos(prog) * startWidth.z;
            Vector3 start = new Vector3(xOffset, 0f, zOffzet);
            starts[i] = start;
            //Debug.Log("i: " + i + " Progress: " + prog + " Direction: " + direction);
        }

        return starts;
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
            float yMagic = 1f - (Mathf.Abs(verticalAngle) / 90f);
            yMagic = Mathf.Sin(yMagic);
            float xOffset = Mathf.Sin(prog) * yMagic;
            float yOffset = (verticalAngle/90f);
            float zOffzet = Mathf.Cos(prog) * yMagic;
            Vector3 direction = new Vector3(xOffset, yOffset, zOffzet);
            directions[i] = direction.normalized;
            //Debug.Log("i: " + i + " Progress: " + prog + " Direction: " + direction);
        }

        return directions;
    }
}
