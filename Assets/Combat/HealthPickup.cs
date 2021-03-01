using Combat;
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

        Character fighter = other.GetComponent<Character>();
        if (fighter == null)
        {
            return;
        }

        if (fighter.Health.Value == fighter.Health.MaxValue)
        {
            return;
        }

        fighter.Health.SetCurrent(fighter.Health.Value + 1);

        foreach (GameObject obj in DisappearanceObjects)
        {
            Instantiate(obj, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}
