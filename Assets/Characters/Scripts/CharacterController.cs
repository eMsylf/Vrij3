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
                return playerController;
            }
            private set { playerController = value; }
        }

        [SerializeField]
        private AIController aIController;
        public AIController AIController
        {
            get
            {
                if (aIController == null)
                {
                    aIController = GetComponent<AIController>();
                    if (aIController == null)
                        aIController = gameObject.AddComponent<AIController>();
                }
                return aIController;
            }
        }

        [Header("Optional components")]
        public Movement movement;
        public Attacking attacking;
        public Targeting targeting;

        private void Update()
        {
            UpdatePlayerControllerStatus();

            UpdateAIControllerStatus();
        }

        public void AssignPlayerControl(PlayerController player)
        {
            PlayerControlled = true;
            PlayerController = player;
            if (!PlayerController.ControlledCharacters.Contains(this))
            {
                PlayerController.AssumePlayerControl(this);
            }
        }

        public void ReleasePlayerControl()
        {
            PlayerControlled = false;
            PlayerController = null;
            if (PlayerController.ControlledCharacters.Contains(this))
            {
                PlayerController.RevokePlayerControl(this);
            }
        }

        /// <summary>
        /// Enables/adds and assigns a player controller when the character is player controlled, and otherwise releases player control.
        /// </summary>
        public void UpdatePlayerControllerStatus()
        {
            if (PlayerControlled && PlayerController == null)
            {
                // If the character is controlled by a player, but no PlayerController is assigned, find player controller in scene
                PlayerController pc = FindObjectOfType<PlayerController>();
                if (pc == null)
                {
                    // If no player controller is found, create a new player controller
                    pc = new GameObject("Player Controller", typeof(PlayerController)).GetComponent<PlayerController>();
                }
                AssignPlayerControl(pc);
            }
            else if (PlayerController != null && PlayerController.enabled)
            {
                // If the character is NOT player-controlled, release the character from player control
                ReleasePlayerControl();
            }
        }

        /// <summary>
        /// Disables the AI controller when player control is enabled, otherwise enables the AI controller
        /// </summary>
        public void UpdateAIControllerStatus()
        {
            if (PlayerControlled && AIController.enabled)
            {
                AIController.enabled = false;
            }
            else if (!AIController.enabled)
            {
                AIController.enabled = true;
            }
        }
    }
}