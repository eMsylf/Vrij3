using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    public CharacterProfile characterProfile;

    [Header("Required components")]
    public Stat health;

    [Header("Optional components")]
    public Stat stamina;
    public Movement movement;

    public UnityEvent die;
    public UnityEvent revive;
}
