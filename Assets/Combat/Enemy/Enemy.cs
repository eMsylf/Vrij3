using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    public class Enemy : Fighter
    {
        public enum States
        {
            Idle,
            Wander,
            FollowSingleTarget,
            Attack
        }
    }
}