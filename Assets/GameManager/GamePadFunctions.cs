using BobJeltes.StandardUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamePadFunctions : Singleton<GamePadFunctions>
{
    public void DoGamepadRumble(float duration = .25f)
    {
        Instance.StartCoroutine(GamepadRumble(duration));
    }

    public IEnumerator GamepadRumble(float duration)
    {
        if (Gamepad.current == null)
        {
            yield break;
        }

        Gamepad.current.SetMotorSpeeds(0.25f, 0.75f);
        yield return new WaitForSeconds(duration);
        Gamepad.current.SetMotorSpeeds(0f, 0f);
    }

    private void OnDisable()
    {
        UnityEngine.InputSystem.Utilities.ReadOnlyArray<Gamepad> all = Gamepad.all;
        for (int i = 0; i < all.Count; i++)
        {
            Gamepad gamepad = all[i];
            gamepad.SetMotorSpeeds(0f, 0f);
        }
        StopAllCoroutines();
    }
}
