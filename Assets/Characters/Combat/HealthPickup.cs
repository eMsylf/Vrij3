using BobJeltes.Extensions;
using RanchyRats.Gyrus;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int HealthPoints = 1;
    public LayerMask InteractingLayers;
    public List<GameObject> DisappearanceObjects = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (InteractingLayers != (InteractingLayers.value | (1 << other.gameObject.layer)))
        {
            //Debug.Log(name + " hit " + other.name + " on ignored layer: " + other.gameObject.layer, this);
            //Debug.DrawLine(transform.position, other.transform.position, Color.red, 2f);
            return;
        }

        Character character = other.GetComponent<Character>();
        if (character == null)
        {
            return;
        }

        if (character.health.Value == character.health.MaxValue)
        {
            return;
        }

        character.health.Value = character.health.Value + 1;

        Extensions.InstantiateList(DisappearanceObjects, transform);

        gameObject.SetActive(false);
    }
}
