using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RanchyRats.Gyrus
{
    [RequireComponent(typeof(Character))]
    public class CharacterController : MonoBehaviour
    {
        private Character character;
        public Character Character
        {
            get
            {
                if (character == null)
                    character = GetComponent<Character>();
                return character;
            }
        }

        public bool PlayerControlled;

        public Movement movement;
        public Attacking attacking;
        public Targeting targeting;

        public virtual void OnEnable()
        {
            
        }
    }
}