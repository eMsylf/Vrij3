using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class Enemy : Fighter
    {
        public AnimationCurve AttackCurve;

        private void OnTriggerEnter(Collider other)
        {
            Fighter otherFighter = other.gameObject.GetComponent<Fighter>();
            if (otherFighter != null)
            {
                if (TouchDamage != 0)
                    otherFighter.TakeDamage(TouchDamage);
            }
        }
    }
}