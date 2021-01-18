using BobJeltes.StandardUtilities;
using FMODUnity;
using System;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : Singleton<AudioSettings>
{
    FMOD.Studio.EventInstance SFXVolumeTestEvent;

    

    public AudioSetting sfx = new AudioSetting("SFX");
    public AudioSetting music = new AudioSetting("Music");
    public AudioSetting master = new AudioSetting("Master");

    private void Awake()
    {
        sfx.audioBus = FMODUnity.RuntimeManager.GetBus("bus:/" + sfx.name);
        music.audioBus = FMODUnity.RuntimeManager.GetBus("bus:/"+ music.name);
        master.audioBus = FMODUnity.RuntimeManager.GetBus("bus:/");

        sfx.Init();
        music.Init();
        master.Init();
    }

    private void Start()
    {
        sfx.Init();
        music.Init();
        master.Init();
    }

    public void Set(AudioSetting setting, float value)
    {
        setting.Set(value);
    }

    public void SetMaster(float value)
    {
        if (Instance == null) return;
        Instance.master.audioBus.setVolume(value);

        PlayerPrefs.SetFloat(Instance.master.name, value);
        PlayerPrefs.Save();
    }

    public void SetMusic(float value)
    {
        if (Instance == null) return;
        Instance.music.audioBus.setVolume(value);
        
        PlayerPrefs.SetFloat(Instance.music.name, value);
        PlayerPrefs.Save();
    }

    public void SetSFX(float value)
    {
        if (Instance == null) return;
        Instance.sfx.audioBus.setVolume(value);

        PlayerPrefs.SetFloat(Instance.sfx.name, value);
        PlayerPrefs.Save();
    }

    [System.Serializable]
    public class AudioSetting
    {
        public string name;
        public Slider slider;
        [Range(0f, 1f)]
        public float defaultVolume = .5f;
        internal FMOD.Studio.Bus audioBus;

        public AudioSetting(string _name)
        {
            name = _name;
        }

        internal void Init()
        {
            if (!PlayerPrefs.HasKey(name))
            {
                PlayerPrefs.SetFloat(name, defaultVolume);
                PlayerPrefs.Save();
            }
            else
            {
                Set(PlayerPrefs.GetFloat(name));
            }
        }

        internal void Set(float value)
        {
            audioBus.setVolume(value);
            PlayerPrefs.SetFloat(name, value);
            PlayerPrefs.Save();
            if (slider == null)
                Debug.LogWarning("Audio setting " + name + " has no slider assigned", Instance.gameObject);
            else
                slider.value = value;
        }
    }
}
