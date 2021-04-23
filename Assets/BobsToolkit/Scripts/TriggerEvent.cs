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
    private GameObject latestEntered;
    public enum EventObject
    {
        This,
        Other
    }

    private void OnTriggerEnter(Collider other)
    {
        if (layers == (layers.value | (1 << other.gameObject.layer)))
            Enter(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (layers == (layers.value | (1 << other.gameObject.layer)))
            Exit(other.gameObject);
    }

    private void Enter(GameObject obj)
    {
        switch (passedEventObject)
        {
            case EventObject.This:
                onTriggerEnter.Invoke(gameObject);
                break;
            case EventObject.Other:
                onTriggerEnter.Invoke(obj);
                break;
            default:
                break;
        }
        latestEntered = gameObject;
    }

    private void Exit(GameObject obj)
    {
        switch (passedEventObject)
        {
            case EventObject.This:
                onTriggerExit.Invoke(gameObject);
                break;
            case EventObject.Other:
                onTriggerExit.Invoke(obj);
                break;
            default:
                break;
        }
        if (obj == latestEntered)
            latestEntered = null;
    }

    private void OnDisable()
    {
        if (onDisableTriggerExit && latestEntered != null)
            Exit(latestEntered);
    }
}
