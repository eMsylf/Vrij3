using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public class Enemy : Fighter
    {
        public UnityEvent OnAttackAnnouncement;
        public void AnnounceAttack()
        {
            OnAttackAnnouncement.Invoke();
        }
    }
}