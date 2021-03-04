using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Combat;

namespace RanchyRats.Gyrus
{
    public partial class PlayerController : MonoBehaviour
    {
        Controls controls;
        public Controls Controls
        {
            get
            {
                if (controls == null)
                {
                    controls = new Controls();
                }
                return controls;
            }
        }

        [SerializeField]
        private List<CharacterController> controlledCharacters = new List<CharacterController>();
        public List<CharacterController> ControlledCharacters
        {
            get => controlledCharacters;
            private set => controlledCharacters = value;
        }

        protected void OnEnable()
        {
            foreach (CharacterController character in controlledCharacters)
            {
                AssumePlayerControl(character, false);
            }
            Controls.Game.Enable();
            LockCursor(true);
        }

        protected void OnDisable()
        {
            foreach (CharacterController controller in controlledCharacters)
            {
                RevokePlayerControl(controller, false);
            }
            Controls.Game.Disable();
            LockCursor(false);
        }

        private void LockCursor(bool locked)
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                Debug.Log("Editor application is not playing. Unlocking cursor.");
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                return;
            }
#endif
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }

        public void AssumePlayerControl(CharacterController character, bool addToList = true)
        {
            if (addToList)
                controlledCharacters.Add(character);
            if (character.movement != null)
            {
                Controls.Game.Movement.performed += _ => character.movement.SetMoveInput(_.ReadValue<Vector2>());
                Controls.Game.Movement.canceled += _ => character.movement.Stop();
                Controls.Game.Dodge.performed += _ => character.movement.AttemptDodge();
                Controls.Game.Run.started += _ => character.movement.StartRunning();
                Controls.Game.Run.canceled += _ => character.movement.StopRunning();
            }

            if (character.attacking != null) 
            {
                Controls.Game.Attack.performed += _ => character.attacking.AttemptAttackCharge();
                Controls.Game.Attack.canceled += _ => character.attacking.CompleteCharge();
            }

            if (character.targeting != null)
            {
                Controls.Game.LockOn.performed += _ => character.targeting.LockOn(transform.position);
            }

        }

        public void RevokePlayerControl(CharacterController character, bool removeFromList = true)
        {
            if (removeFromList)
                controlledCharacters.Remove(character);
            if (character.movement != null)
            {
                Controls.Game.Movement.performed -= _ => character.movement.SetMoveInput(_.ReadValue<Vector2>());
                Controls.Game.Movement.canceled -= _ => character.movement.Stop();
                Controls.Game.Dodge.performed -= _ => character.movement.AttemptDodge();
                Controls.Game.Run.started -= _ => character.movement.StartRunning();
                Controls.Game.Run.canceled -= _ => character.movement.StopRunning();
            }

            if (character.attacking != null)
            {
                Controls.Game.Attack.performed -= _ => character.attacking.AttemptAttackCharge();
                Controls.Game.Attack.canceled -= _ => character.attacking.CompleteCharge();
            }

            if (character.targeting != null)
            {
                Controls.Game.LockOn.performed -= _ => character.targeting.LockOn(transform.position);
            }
        }

        public void DoGamepadRumble(float duration = .25f)
        {
            GamePadFunctions.Instance.DoGamepadRumble(duration);
        }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            foreach (CharacterController character in controlledCharacters)
            {
                Gizmos.DrawLine(transform.position, character.transform.position);
            }
        }
#endif
    }
}