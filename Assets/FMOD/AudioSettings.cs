using BobJeltes.StandardUtilities;
using UnityEngine;

public class AudioSettings : Singleton<AudioSettings>
{
    FMOD.Studio.EventInstance SFXVolumeTestEvent;

    FMOD.Studio.Bus SFX;
    FMOD.Studio.Bus Music;
    FMOD.Studio.Bus Master;

    public string sfxName = "SFX";
    public string musicName = "Music";
    public string masterName = "master";

    private void Awake()
    {
        SFX = FMODUnity.RuntimeManager.GetBus("bus:/" + sfxName);
        Music = FMODUnity.RuntimeManager.GetBus("bus:/"+ musicName);
        Master = FMODUnity.RuntimeManager.GetBus("bus:/");

        InitVolume(sfxName);
        InitVolume(musicName);
        InitVolume(masterName);
    }

    public void InitVolume(string keyName)
    {
        if (!PlayerPrefs.HasKey(keyName))
        {
            PlayerPrefs.SetFloat(keyName, 50f);
            PlayerPrefs.Save();
        }
        else
        {
            SetMusic(PlayerPrefs.GetFloat(keyName));
        }
    }

    public void SetMaster(float value)
    {
        if (Instance == null) return;
        Instance.Master.setVolume(value);

        //PlayerPrefs.SetFloat("master", value);
        //PlayerPrefs.Save();
    }

    public void SetMusic(float value)
    {
        if (Instance == null) return;
        Instance.Music.setVolume(value);
        
        PlayerPrefs.SetFloat(musicName, value);
        PlayerPrefs.Save();
    }

    public void SetSFX(float value)
    {
        if (Instance == null) return;
        Instance.SFX.setVolume(value);

        PlayerPrefs.SetFloat(sfxName, value);
        PlayerPrefs.Save();
    }
}
