using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinyMoveScript : MonoBehaviour
{
    public float movespeed;
    // Update is called once per frame
    void Update()
    {
        if (transform.position.z >= 54)
        {
            transform.Translate(0, 0, -movespeed);
        }
        else if (transform.position.z <= 45) {
            transform.Translate(0, 0, +movespeed);
        }
    }
}
