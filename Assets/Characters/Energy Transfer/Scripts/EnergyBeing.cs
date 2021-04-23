using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBeing : MonoBehaviour
{
    [Tooltip("Directly dictates how many energy orbs are released upon death")]
    [Min(0)]
    public int Energy = 0;
}
