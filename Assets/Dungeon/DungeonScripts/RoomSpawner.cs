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
    private bool spawned = false;

    void Start()
    {
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplatesScript>();
        Invoke("Spawn", 1f);
    }

    void Spawn()
    {
        for (int i = 0; i < 10; i++)
        {
            if (spawned == false)
            {
                if (openingDirection == 1)
                {
                    // Need to spawn a room with a TOP door
                    rand = Random.Range(0, templates.topRooms.Length);
                    Instantiate(templates.topRooms[rand], transform.position, templates.topRooms[rand].transform.rotation);
                }

                else if (openingDirection == 2)
                {
                    // Need to spawn a room with a BOTTOM door
                    rand = Random.Range(0, templates.bottomRooms.Length);
                    Instantiate(templates.bottomRooms[rand], transform.position, templates.bottomRooms[rand].transform.rotation);
                }

                else if (openingDirection == 3)
                {
                    // Need to spawn a room with a RIGHT door
                    rand = Random.Range(0, templates.rightRooms.Length);
                    Instantiate(templates.rightRooms[rand], transform.position, templates.rightRooms[rand].transform.rotation);
                }

                else if (openingDirection == 4)
                {
                    // Need to spawn a room with a LEFT door
                    rand = Random.Range(0, templates.leftRooms.Length);
                    Instantiate(templates.leftRooms[rand], transform.position, templates.leftRooms[rand].transform.rotation);
                }
                spawned = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other){
        if (other.CompareTag("DungeonSpawnpoint") && other.GetComponent<RoomSpawner>().spawned == true) {
            Destroy(gameObject);
        }
    }

}
