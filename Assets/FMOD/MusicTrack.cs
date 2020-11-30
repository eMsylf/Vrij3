using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MusicTrack
{
    public string name;

    public MusicTrack(string _name)
    {
        name = _name;
    }

    public void AdjustLevel(FMODUnity.StudioEventEmitter musicEmitter, float change)
    {
        musicEmitter.EventInstance.getParameterByName(name, out float current);
        SetLevel(musicEmitter, current + change);
    }

    public void SetLevel(FMODUnity.StudioEventEmitter musicEmitter, float newLevel)
    {
        Debug.Log("Set " + name + " level to " + newLevel);
        musicEmitter.SetParameter(name, newLevel);
        FMOD.RESULT rESULT = musicEmitter.EventInstance.getParameterByName(name, out _);
        Debug.Log("Result: " + rESULT);
    }
}
