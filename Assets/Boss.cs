using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [Header("Boss")]
    public Spawner eyeSpawner;
    public Spawner motmugSpawner;

    void SpawnEye()
    {
        eyeSpawner.Spawn();
    }

    void SpawnMotmugs()
    {
        motmugSpawner.Spawn();
    }
}
