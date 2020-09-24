using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
//    Vector2 worldSize = new Vector2(4, 4);

//    Room[,] rooms;

//    List<Vector2> takenPositions = new List<Vector2>();

//    int gridSizeX, gridSizeY, numberOfRooms = 20;

//    public GameObject roomWhiteObj;

//    private void Start()
//    {
//        if (numberOfRooms >= (worldSize.x * 2) * (worldSize.y * 2)) {
//            numberOfRooms = Mathf.RoundToInt((worldSize.x * 2) * (worldSize.y * 2));
//        }

//        gridSizeX = Mathf.RoundToInt(worldSize.x);
//        gridSizeY = Mathf.RoundToInt(worldSize.y);
//        CreateRooms();
//        SetRoomDoors();
//        DrawMap();
//    }

//    void CreateRooms() {
//        //setup
//        rooms = new Room[gridSizeX * 2, gridSizeY * 2];
//        rooms[gridSizeX, gridSizeY] = new Room(Vector2.zero, 1);
//        takenPositions.Insert(0, Vector2.zero);
//        Vector2 checkPos = Vector2.zero;

//        //magic numbers decides how many times you branch out
//        float randomCompare = 0.2f, randomCompareStart = 0.2f, randomCompareEnd = 0.01f;

//        //add rooms
//        for (int i = 0; i < numberOfRooms -1; i++)
//        {
//            float randomPerc = ((float)i) / (((float)numberOfRooms - 1));
//            randomCompare = Mathf.Lerp(randomCompareStart, randomCompareEnd, randomPerc);
//            //grab new position
//            checkPos = NewPosition();

//            //test the new position, and branch out a little
//            if (NumberOfNeighbors(checkPos, takenPositions) > 1 && UnityEngine.Random.value > randomCompare) {
//                int iterations = 0;

//                do
//                {
//                    checkPos = SelectiveNewPosition();
//                    iterations++;
//                } while (NumberOfNeighbors(checkPos, takenPositions) > 1 && iterations < 100);
//                if (iterations >= 50) print("error: could not create with fewer neighbors than : " +NumberOfNeighbors(checkPos,takenPositions));
//            }
//            //finalize position, 0 represents a normal room
//            rooms[(int)checkPos.x + gridSizeX, (int)checkPos.y + gridSizeY] = new Room(checkPos, 0);
//            takenPositions.Insert(0, checkPos);
//        }
//    }

//    private int NumberOfNeighbors(Vector2 checkPos, List<Vector2> takenPositions)
//    {
//        throw new NotImplementedException();
//    }

//    private Vector2 SelectiveNewPosition()
//    {
//        int index = 0, inc = 0;
//        int x = 0, y = 0;
//    }

//    void SetRoomDoors()
//    {

//    }
//    void DrawMap()
//    {

//    }

//    private Vector2 NewPosition()
//    {

//    }

}
