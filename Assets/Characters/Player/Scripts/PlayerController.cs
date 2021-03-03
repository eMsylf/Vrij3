using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Combat;

namespace RanchyRats.Gyrus
{
    public partial class PlayerController : CharacterController
    {
        #region Julia Added

        [System.Serializable]
        public struct Sounds
        {
            public FMODUnity.StudioEventEmitter attack1Sound;
            public FMODUnity.StudioEventEmitter attack2Sound;
            public FMODUnity.StudioEventEmitter attack3Sound;

            public void PlayAttackSound(int index)
            {
                if (index == 0) attack1Sound.Play();
                else if (index == 1) attack2Sound.Play();
                else if (index == 2) attack3Sound.Play();
            }
        }
        public Sounds sounds;




        //Later to be replaced by FMOD elements For now it's hard coded :^) You can find where I added something using the following indicator VVV
        //---------------------------------------------------------     (What it's about)
        //public

        #endregion
        //public UnityEvent OnHit;

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

        public override void OnEnable()
        {
            base.OnEnable();
            Controls.Game.Enable();
            AssumePlayerControl();

            movement.state = Movement.State.Idle;

            //Debug.Log("Set current health and stamina of " + name + " to max", this);

            LockCursor(true);
            attacking.state = Attacking.State.Ready;
            attacking.ClearChargingProhibitors();
        }

        private void OnDisable()
        {
            //Debug.Log("Player disabled");
            Controls.Game.Disable();
            RevokePlayerControl();

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

        bool playerControlled = false;
        private void AssumePlayerControl()
        {
            if (playerControlled)
                return;
            Controls.Game.Movement.performed += _ => movement.SetMoveInput(_.ReadValue<Vector2>());
            Controls.Game.Movement.canceled += _ => movement.Stop();

            Controls.Game.Dodge.performed += _ => movement.AttemptDodge();

            Controls.Game.Attack.performed += _ => attacking.AttemptAttackCharge();
            Controls.Game.Attack.canceled += _ => attacking.CompleteCharge();

            Controls.Game.LockOn.performed += _ => targeting.LockOn(transform.position);

            Controls.Game.Run.started += _ => movement.StartRunning();
            Controls.Game.Run.canceled += _ => movement.StopRunning();

            playerControlled = true;
        }

        private void RevokePlayerControl()
        {
            if (!playerControlled)
                return;
            Controls.Game.Movement.performed -= _ => movement.SetMoveInput(_.ReadValue<Vector2>());
            Controls.Game.Movement.canceled -= _ => movement.Stop();

            Controls.Game.Dodge.performed -= _ => movement.AttemptDodge();

            Controls.Game.Attack.performed -= _ => attacking.AttemptAttackCharge();
            Controls.Game.Attack.canceled -= _ => attacking.CompleteCharge();

            Controls.Game.LockOn.performed -= _ => targeting.LockOn(transform.position);

            Controls.Game.Run.started -= _ => movement.StartRunning();
            Controls.Game.Run.canceled -= _ => movement.StopRunning();

            playerControlled = false;
        }

        public void DoGamepadRumble(float duration = .25f)
        {
            GamePadFunctions.Instance.DoGamepadRumble(duration);
        }
    }
}