using BobJeltes.StandardUtilities;

public class AudioSettings : Singleton<AudioSettings>
{
    FMOD.Studio.EventInstance SFXVolumeTestEvent;

    FMOD.Studio.Bus SFX;
    FMOD.Studio.Bus Music;
    FMOD.Studio.Bus Master;

    private void Awake()
    {
        Master = FMODUnity.RuntimeManager.GetBus("bus:/");
        Music = FMODUnity.RuntimeManager.GetBus("bus:/Music");
        SFX = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
    }

    public void SetMaster(float value)
    {
        if (Instance == null) return;
        Instance.Master.setVolume(value);
    }

    public void SetMusic(float value)
    {
        if (Instance == null) return;
        Instance.Music.setVolume(value);
    }

    public void SetSFX(float value)
    {
        if (Instance == null) return;
        Instance.SFX.setVolume(value);
    }
}
