using UnityEngine;
using BobJeltes.Events;

public class TriggerEvent : MonoBehaviour
{
    public LayerMask layers;
    [Tooltip("The game object that is passed with the events listed below")]
    public TriggerObject triggerObject;
    public UnityEventGameObject onTriggerEnter;
    public UnityEventGameObject onTriggerExit;
    public bool onDisableTriggerExit;
    public enum TriggerObject
    {
        This,
        Other
    }

    private void OnTriggerEnter(Collider other)
    {
        if (layers != (layers.value | (1 << other.gameObject.layer)))
        {
            return;
        }
        Debug.Log(other.name + " entered trigger of " + name, this);
        switch (triggerObject)
        {
            case TriggerObject.This:
                onTriggerEnter.Invoke(gameObject);
                break;
            case TriggerObject.Other:
                onTriggerEnter.Invoke(other.gameObject);
                break;
            default:
                break;
        }
        latestEntered = gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (layers != (layers.value | (1 << other.gameObject.layer)))
        {
            return;
        }
        Debug.Log(other.name + " left trigger of " + name, this); 
        switch (triggerObject)
        {
            case TriggerObject.This:
                onTriggerExit.Invoke(gameObject);
                break;
            case TriggerObject.Other:
                onTriggerExit.Invoke(other.gameObject);
                break;
            default:
                break;
        }
    }

    private GameObject latestEntered;

    private void OnDisable()
    {
        if (onDisableTriggerExit)
        {
            switch (triggerObject)
            {
                case TriggerObject.This:
                    onTriggerExit.Invoke(gameObject);
                    break;
                case TriggerObject.Other:
                    onTriggerExit.Invoke(latestEntered);
                    break;
                default:
                    break;
            }
        }
    }
}
