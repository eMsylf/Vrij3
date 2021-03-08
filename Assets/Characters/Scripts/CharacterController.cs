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
        public Movement movement;
        public Attacking attacking;
        public Targeting targeting;

        private void Update()
        {
            CheckPlayerControllerStatus();

            if (!PlayerControlled)
            {

            }
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

        public void CheckPlayerControllerStatus()
        {
            if (PlayerControlled)
            {
                if (PlayerController == null)
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