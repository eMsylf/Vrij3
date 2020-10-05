using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public DungeonRoom RoomPrefab;

    [Min(1)]
    public int MinRooms = 3;
    [Min(1)]
    public int MaxRooms = 20;
    [Tooltip("This variable influences the number of rooms. \n0 = small dungeons. \n1 = large dungeons.")]
    [Range(0f, 1f)]
    public float Weight;

    [Min(10)]
    public int DungeonHeight = 10;
    [Min(10)]
    public int DungeonWidth = 10;

    [SerializeField]
    private List<DungeonRoom> DungeonRooms = new List<DungeonRoom>();
    [SerializeField]
    private Dictionary<int[,], DungeonRoom> coordinateRoomPairs = new Dictionary<int[,], DungeonRoom>();

    // False = free, True = occupied
    bool[,] coords = new bool[20, 20];

    public void Generate()
    {
        DestroyDungeon();

        int i;
        for (i = 0; i < MaxRooms; i++)
        {
            // Check if minimum number of rooms has been reached
            if (i >= MinRooms)
            {
                // Random number to determine if generation should be finalized
                if (UnityEngine.Random.value >= Weight)
                {
                    break;
                }
            }

            // Generate a room
            Debug.Log("Generate dungeon room " + i);
            DungeonRoom room = Instantiate(RoomPrefab, transform).GetComponent<DungeonRoom>();
            room.name += "(Room " + i + ")";
            DungeonRooms.Add(room);
        }
        //PlaceRooms();
        FinalizeDungeon(i);
    }

    public void PlaceRooms()
    {
        Vector3 roomBounds = RoomPrefab.GetComponent<MeshRenderer>().bounds.extents;

        coords = new bool[DungeonWidth, DungeonHeight];

        for (int i = 0; i < DungeonWidth; i++)
        {
            for (int j = 0; j < DungeonHeight; j++)
            {
                Debug.Log("Coordinate (" + i + ", " + j + ") has a value of " + coords[i,j]);
            }
        }

        Vector2Int originCoordinate = new Vector2Int(DungeonWidth / 2, DungeonHeight / 2);
        Vector2Int currentCoordinate = originCoordinate;
        Debug.Log("Placing rooms");

        DungeonRoom originRoom = DungeonRooms[0];
        coords[originCoordinate.x, originCoordinate.y] = true;

        for (int i = 1; i < DungeonRooms.Count; i++)
        {
            DungeonRoom room = DungeonRooms[i];

            // Check for a free adjacent coordinate
            Debug.Log("Finding a spot for room " + i);
            if (FindFreeCoordinateAdjacentTo(currentCoordinate, out Vector2Int freeSpot))
            {
                // Assign the discovered free position
                Vector3 offset = new Vector3((originCoordinate.x + freeSpot.x) * roomBounds.x, transform.position.y, (originCoordinate.y + freeSpot.y * roomBounds.z));
                Vector3 position = transform.position + offset;
                room.transform.position = position;
                coords[freeSpot.x, freeSpot.y] = true;
            }
            currentCoordinate = freeSpot;
        }
    }

    Vector2Int[] directions = { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(1, 0 ), new Vector2Int(-1, 0 ) };

    private bool FindFreeCoordinateAdjacentTo(Vector2Int coordinate, out Vector2Int freeCoordinate)
    {
        //Debug.Log("Trying to find a free neighbor from " + coordinate);
        for (int i = 0; i < directions.Length; i++)
        {
            if (!CheckCoordinateAvailability(coordinate + directions[i]))
            {
                freeCoordinate = coordinate + directions[i];
                
                return true;
            }
        }
        freeCoordinate = coordinate + directions[UnityEngine.Random.Range(0, 4)];
        Debug.Log("Did not find a free coordinate next to " + coordinate + ". Check next from " + freeCoordinate);
        return false;
    }

    public bool CheckCoordinateAvailability(Vector2Int coordinate)
    {
        //Debug.Log("Checked coordinate " + coordinate + ". World equivalent: ");
        if (coordinate.x > DungeonWidth)
            Debug.LogError("Out of bounds of dungeon width: " + coordinate.x  + " / " + DungeonWidth);
        if (coordinate.y > DungeonHeight)
            Debug.LogError("Out of bounds of dungeon width: " + coordinate.y  + " / " + DungeonHeight);

        return coords[coordinate.x, coordinate.y];
    }

    public void FinalizeDungeon(int rooms)
    {
        Debug.Log("Finalize dungeon. Number of rooms: " + rooms);
    }



    public void DestroyDungeon()
    {
        while (transform.childCount > 0)
        {
            Debug.Log("Destroyed " + transform.GetChild(0).gameObject.name);
#if UNITY_EDITOR
            DestroyImmediate(transform.GetChild(0).gameObject);
#else
            Destroy(transform.GetChild(0).gameObject);
#endif
        }
        DungeonRooms.Clear();
    }

#if UNITY_EDITOR
    public void OnDrawGizmosSelected()
    {
        Vector3 roomBounds = RoomPrefab.GetComponent<MeshRenderer>().bounds.extents;
        Gizmos.DrawWireCube(transform.position, new Vector3(roomBounds.x * DungeonWidth, roomBounds.y, roomBounds.z * DungeonHeight));
    }
#endif
}
