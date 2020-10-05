using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Wall : MonoBehaviour
{
    public GameObject Full;
    public GameObject LeftPiece;
    public GameObject RightPiece;
    public GameObject Door;

    private bool hasOpening = false;
    private bool doorOpen = false;

    public void ToggleDoor()
    {
        doorOpen = Door.activeInHierarchy;
        OpenDoor(doorOpen);
    }

    public void OpenDoor(bool open)
    {
        if (hasOpening)
        {
            Door.SetActive(!open);
        }
        else
        {
            Debug.Log("This wall " + name + " has no door. Enable its opening before opening the door again.", this);
        }
    }


    public void ToggleOpening()
    {
        hasOpening = Full.activeInHierarchy;
        HasOpening(hasOpening);
    }

    public void HasOpening(bool hasOpening)
    {
        Door.SetActive(hasOpening);
        LeftPiece.SetActive(hasOpening);
        RightPiece.SetActive(hasOpening);

        Full.SetActive(!hasOpening);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
