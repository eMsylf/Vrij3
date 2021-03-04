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
        public List<CharacterController> controlledCharacters = new List<CharacterController>();

        protected void OnEnable()
        {
            foreach (CharacterController character in controlledCharacters)
            {
                AssumePlayerControl(character);
            }
            Controls.Game.Enable();
            LockCursor(true);
        }

        protected void OnDisable()
        {
            foreach (CharacterController controller in controlledCharacters)
            {
                RevokePlayerControl(controller);
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

        public void AssumePlayerControl(CharacterController controller)
        {
            if (controller.movement != null)
            {
                Controls.Game.Movement.performed += _ => controller.movement.SetMoveInput(_.ReadValue<Vector2>());
                Controls.Game.Movement.canceled += _ => controller.movement.Stop();
                Controls.Game.Dodge.performed += _ => controller.movement.AttemptDodge();
                Controls.Game.Run.started += _ => controller.movement.StartRunning();
                Controls.Game.Run.canceled += _ => controller.movement.StopRunning();
            }

            if (controller.attacking != null) 
            {
                Controls.Game.Attack.performed += _ => controller.attacking.AttemptAttackCharge();
                Controls.Game.Attack.canceled += _ => controller.attacking.CompleteCharge();
            }

            if (controller.targeting != null)
            {
                Controls.Game.LockOn.performed += _ => controller.targeting.LockOn(transform.position);
            }

        }

        public void RevokePlayerControl(CharacterController controller)
        {
            if (controller.movement != null)
            {
                Controls.Game.Movement.performed -= _ => controller.movement.SetMoveInput(_.ReadValue<Vector2>());
                Controls.Game.Movement.canceled -= _ => controller.movement.Stop();
                Controls.Game.Dodge.performed -= _ => controller.movement.AttemptDodge();
                Controls.Game.Run.started -= _ => controller.movement.StartRunning();
                Controls.Game.Run.canceled -= _ => controller.movement.StopRunning();
            }

            if (controller.attacking != null)
            {
                Controls.Game.Attack.performed -= _ => controller.attacking.AttemptAttackCharge();
                Controls.Game.Attack.canceled -= _ => controller.attacking.CompleteCharge();
            }

            if (controller.targeting != null)
            {
                Controls.Game.LockOn.performed -= _ => controller.targeting.LockOn(transform.position);
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