using RanchyRats.Gyrus;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    private List<Room> rooms = new List<Room>();
    public bool DeactivateRoomsOnStart = true;
    public float RoomDespawnDistance = 20f;
    public float CheckDistanceEvery = 3f;
    private float check = 0f;

    void Start()
    {
        rooms = GetComponentsInChildren<Room>().ToList();
        if (DeactivateRoomsOnStart)
        {
            foreach (Room room in rooms)
            {
                room.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (check < CheckDistanceEvery)
        {
            check += Time.unscaledDeltaTime;
        }
        else
        {
            Debug.Log("Check room despawns");
            foreach (Room room in rooms)
            {
                room.gameObject.SetActive(Vector3.Distance(FindObjectOfType<PlayerController>().transform.position, room.transform.position) < RoomDespawnDistance);
            }
            check = 0f;
        }
    }
}
