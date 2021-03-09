using RanchyRats.Gyrus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitDeath : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Character character = other.attachedRigidbody.GetComponent<Character>();

        if (character != null)
        {
            character.Die();
        }
    }
}
