using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    public void ActivatePlayer()
    {
        GameManager.Instance.ActivatePlayer();
    }
}
