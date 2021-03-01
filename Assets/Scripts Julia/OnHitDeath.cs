using RanchyRats.Gyrus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHitDeath : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController playerController = other.attachedRigidbody.GetComponent<PlayerController>();

        if (playerController != null)
        {
            playerController.Die();
        }
    }
}
