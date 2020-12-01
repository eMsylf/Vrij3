using BobJeltes.StandardUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettings : Singleton<AudioSettings>
{
    FMOD.Studio.EventInstance SFXVolumeTestEvent;

    //FMOD.Studio.Bus SFX;
    //FMOD.Studio.Bus Music;
    FMOD.Studio.Bus Master;

    //float MusicVolume = .5f;
    //float SFXVolume = .5f;
    //float MasterVolume = .5f;

    private void Awake()
    {
        Master = FMODUnity.RuntimeManager.GetBus("bus:/Master/Fader");
    }

    public void SetMaster(float value)
    {
        Instance.SetMasterPrivate(value);
    }

    private void SetMasterPrivate(float value)
    {
        Master.setVolume(value);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
