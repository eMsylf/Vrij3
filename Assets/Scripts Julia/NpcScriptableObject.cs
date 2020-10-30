using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NPC", menuName = "NPC's/NPC")]
public class NpcScriptableObject : ScriptableObject
{
    public new string name;
    public string age;
    public string job;

    public Sprite image;
    public Color imageColor;

    public int health;
    public int stamina;
    public float chargeSpeed;
    public float maxDamage;
}
