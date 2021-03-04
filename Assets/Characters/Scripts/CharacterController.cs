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

        [SerializeField]
        private bool PlayerControlled;
        [SerializeField]
        private PlayerController playerController = null;
        public PlayerController PlayerController
        {
            get
            {
                //if (playerController == null)
                //    GetComponent<PlayerController>();
                return playerController;
            }
            private set { playerController = value; }
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

        public void AssignPlayerControl(PlayerController player)
        {
            // TODO: Add functionality
            PlayerControlled = true;
            PlayerController = player;
            if (!PlayerController.controlledCharacters.Contains(this))
            {
                PlayerController.AssumePlayerControl(this);
            }
        }

        public void ReleasePlayerControl()
        {
            // TODO: Add functionality
            PlayerControlled = false;
            PlayerController = null;
            if (PlayerController.controlledCharacters.Contains(this))
            {
                PlayerController.RevokePlayerControl(this);
            }
        }

        public void CheckPlayerControllerStatus()
        {
            if (PlayerControlled)
            {
                if (PlayerController == null)
                {
                    // If the character is controlled by a player, but no PlayerController is assigned, Find player controller in scene
                    PlayerController pc = FindObjectOfType<PlayerController>();
                    if (pc != null)
                    {
                        AssignPlayerControl(pc);
                    }
                }
            }
            else
            {
                if (PlayerController != null && PlayerController.enabled)
                {
                    // If the character is NOT player-controlled, disable player controller
                    ReleasePlayerControl();
                }
            }
        }
    }
}