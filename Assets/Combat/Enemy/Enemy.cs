using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class Enemy : Fighter
    {
        private void OnCollisionEnter(Collision collision)
        {
            Fighter otherFighter = collision.gameObject.GetComponent<Fighter>();
            if (otherFighter != null)
            {
                if (TouchDamage != 0)
                {
                    otherFighter.TakeDamage(TouchDamage, TouchDamageInvincibilityTime, this);
                    Debug.Log(otherFighter + " takes touch damage");
                }
            }
        }
    }
}