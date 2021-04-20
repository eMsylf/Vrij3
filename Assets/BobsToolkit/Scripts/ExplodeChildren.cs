using BobJeltes.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplodeChildren : MonoBehaviour
{
    public float Strength = 1f;
    public ForceMode forceMode = ForceMode.Impulse;

    private void OnEnable()
    {
        Explode();
    }

    private void Start()
    {
        Explode();
    }

    public void Explode()
    {
        List<Rigidbody> childRbs = GetComponentsInChildren<Rigidbody>().ToList();

        foreach (Rigidbody childRb in childRbs)
        {
            childRb.AddForce(Extensions.RandomVector301(), forceMode);
        }
    }
}
