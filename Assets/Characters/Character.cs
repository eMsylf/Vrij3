using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("Required components")]
    public CharacterProfile characterProfile;

    public struct Modifiers
    {
        public float energy;
        public float health;
        public float speed;
        public float attackFrequency;
        public float size;
    }
    public Modifiers modifiers;

    [Header("Optional components")]
    public Stat health;
    public Stat stamina;
    public Movement movement;

    public UnityEvent die;
    public UnityEvent revive;
}
