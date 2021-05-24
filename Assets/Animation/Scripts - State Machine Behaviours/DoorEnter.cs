using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorEnter : MonoBehaviour
{
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

    }

    void OnTriggerEnter(Collider col)
    {
        anim.Play("AngerDoor_Open");
    }
    
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
