using UnityEngine;
using BobJeltes.Events;

public class TriggerEvent : MonoBehaviour
{
    public LayerMask layers;
    [Tooltip("The game object that is passed with the events listed below")]
    public EventObject passedEventObject;
    public UnityEventGameObject onTriggerEnter;
    public UnityEventGameObject onTriggerExit;
    public bool onDisableTriggerExit;
    public enum EventObject
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
        //Debug.Log(other.name + " entered trigger of " + name, this);
        switch (passedEventObject)
        {
            case EventObject.This:
                onTriggerEnter.Invoke(gameObject);
                break;
            case EventObject.Other:
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
        switch (passedEventObject)
        {
            case EventObject.This:
                onTriggerExit.Invoke(gameObject);
                break;
            case EventObject.Other:
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
            switch (passedEventObject)
            {
                case EventObject.This:
                    onTriggerExit.Invoke(gameObject);
                    break;
                case EventObject.Other:
                    onTriggerExit.Invoke(latestEntered);
                    break;
                default:
                    break;
            }
        }
    }
}
