using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public int openingDirection;
    // 1 = top opening
    // 2 = bottom opening
    // 3 = right opening
    // 4 = left opening


    private RoomTemplatesScript templates;
    private int rand;
    public bool spawned = false;
    //public GameObject masterRoom;

    void Start()
    {
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplatesScript>();
        if (spawned == false)
        {
            Invoke("Spawn", 1f);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DungeonSpawnpoint") && other.GetComponent<RoomSpawner>().spawned == true)
        {
            Destroy(gameObject);
        }
    }
    void Spawn()
    {

                if (openingDirection == 1)
                {
                // Is a top door and need to spawn a room with a BOTTOM door
                rand = Random.Range(0, templates.bottomRooms.Length);
                Instantiate(templates.bottomRooms[rand], transform.position, templates.bottomRooms[rand].transform.rotation);
            }

                else if (openingDirection == 2)
                {
                // Need to spawn a room with a TOP door
                rand = Random.Range(0, templates.topRooms.Length);
                Instantiate(templates.topRooms[rand], transform.position, templates.topRooms[rand].transform.rotation);
            }

                else if (openingDirection == 3)
                {
                // Need to spawn a room with a LEFT door
                rand = Random.Range(0, templates.leftRooms.Length);
                Instantiate(templates.leftRooms[rand], transform.position, templates.leftRooms[rand].transform.rotation);            
            }

                else if (openingDirection == 4)
                {
                // Need to spawn a room with a LEFT door
                rand = Random.Range(0, templates.rightRooms.Length);
                Instantiate(templates.rightRooms[rand], transform.position, templates.rightRooms[rand].transform.rotation);
            }
        spawned = true;

    }

}
