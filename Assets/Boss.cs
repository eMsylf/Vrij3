﻿using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [Header("Boss")]
    public GameObjectEmitter ScreamEmitter;
    public Spawner eyeSpawner;
    public Spawner motmugSpawner;

    [Header("Boss animation")]
    public Animator animator;

    void SpawnScream()
    {
        ScreamEmitter.Emit();
    }

    void SpawnEye()
    {
        eyeSpawner.Spawn();
    }

    void SpawnMotmugs()
    {
        motmugSpawner.Spawn();
    }

    public override void Die()
    {
        base.Die();
        StartDeathAnimation();
    }

    public void StartDeathAnimation()
    {
        if (animator == null)
        {
            Debug.LogError("Animator not assigned");
            return;
        }

        animator.SetTrigger("Death");
    }
}
