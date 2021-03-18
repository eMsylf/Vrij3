using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
            LockCursor();
        }

        protected void OnDisable()
        {
            foreach (CharacterController controller in controlledCharacters)
            {
                RevokePlayerControl(controller, false);
            }
            Controls.Game.Disable();
            UnlockCursor();
        }

        public void LockCursor() => DoCursorLock(true);

        public void UnlockCursor() => DoCursorLock(false);

        private void DoCursorLock(bool enabled)
        {
#if UNITY_EDITOR
            // Failsafe editor cursor check to prevent cursor from locking at unintended moments
            if (!EditorApplication.isPlaying)
            {
                Debug.Log("Editor application is not playing. Unlocking cursor.");
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                return;
            }
#endif
            Cursor.visible = !enabled;
            Cursor.lockState = enabled ? CursorLockMode.Locked : CursorLockMode.None;
        }

        public void AssumePlayerControl(CharacterController character, bool addToList = true)
        {
            if (controlledCharacters.Contains(character))
            {
                Debug.Log(character.name + " already controlled by player controller", this);
                return;
            }

            if (addToList) controlledCharacters.Add(character);

            if (character.movement != null)
            {
                Controls.Game.Movement.performed += _ =>    character.movement.SetMoveInput(_.ReadValue<Vector2>());
                Controls.Game.Movement.canceled += _ =>     character.movement.Stop();
                Controls.Game.Dodge.performed += _ =>       character.movement.AttemptDodge();
                Controls.Game.Run.started += _ =>           character.movement.StartRunning();
                Controls.Game.Run.canceled += _ =>          character.movement.StopRunning();
            }

            if (character.attacking != null)
            {
                Controls.Game.Attack.performed += _ =>      character.attacking.AttemptAttackCharge();
                Controls.Game.Attack.canceled += _ =>       character.attacking.EndCharge(true);
            }

            if (character.targeting != null)
                Controls.Game.LockOn.performed += _ =>      character.targeting.LockOn(transform.position);

        }

        public void RevokePlayerControl(CharacterController character, bool removeFromList = true)
        {
            if (character == null || controlledCharacters.Contains(character))
            {
                Debug.Log(character.name + " is not being controlled by player controller", this);
                return;
            }

            if (removeFromList) controlledCharacters.Remove(character);

            if (character.movement != null)
            {
                Controls.Game.Movement.performed -= _ =>    character.movement.SetMoveInput(_.ReadValue<Vector2>());
                Controls.Game.Movement.canceled -= _ =>     character.movement.Stop();
                Controls.Game.Dodge.performed -= _ =>       character.movement.AttemptDodge();
                Controls.Game.Run.started -= _ =>           character.movement.StartRunning();
                Controls.Game.Run.canceled -= _ =>          character.movement.StopRunning();
            }

            if (character.attacking != null)
            {
                Controls.Game.Attack.performed -= _ =>      character.attacking.AttemptAttackCharge();
                Controls.Game.Attack.canceled -= _ =>       character.attacking.EndCharge(true);
            }

            if (character.targeting != null)
                Controls.Game.LockOn.performed -= _ =>      character.targeting.LockOn(transform.position);
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