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
        [SerializeField]
        private PlayerController playerController;
        public PlayerController PlayerController
        {
            get
            {
                if (playerController == null)
                    GetComponent<PlayerController>();
                return playerController;
            }
        }

        public Movement movement;
        public Attacking attacking;
        public Targeting targeting;

        protected virtual void OnEnable()
        {
            
        }

        protected virtual void OnDisable()
        {

        }

        private void Update()
        {
            CheckPlayerControllerStatus();
        }

        public void AssignPlayerControl()
        {
            // TODO: Add functionality
        }

        public void RelinquishPlayerControl()
        {
            // TODO: Add functionality
        }

        public void CheckPlayerControllerStatus()
        {
            if (PlayerControlled)
            {
                if (PlayerController == null)
                {
                    // Add player controller
                }
                else
                {
                    // Enable player controller
                }
            }
            else
            {
                if (PlayerController.enabled)
                {
                    // Disable player controller
                }
            }
        }
    }
}