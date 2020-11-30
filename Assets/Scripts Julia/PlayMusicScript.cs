using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BobJeltes.StandardUtilities;

public class PlayMusicScript : Singleton<PlayMusicScript>
{
    public FMODUnity.StudioEventEmitter musicEmitter;
    public string fallbackEventName = "event:/DUNGEON1";
    public FMODUnity.StudioEventEmitter GetMusicEmitter()
    {
        if (musicEmitter == null)
        {
            musicEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
            if (musicEmitter == null) { 
                musicEmitter = gameObject.AddComponent<FMODUnity.StudioEventEmitter>();
                musicEmitter.Event = fallbackEventName;
                if (string.IsNullOrWhiteSpace(fallbackEventName))
                {
                    Debug.LogError("Fallback event name not set", this);
                }
            }
        }
        return musicEmitter;
    }

    void Start()
    {
        if (Instance.GetMusicEmitter() == null) Debug.LogError("MusicEmitter is missing", this);
        else Instance.musicEmitter.Play();
    }

    public MusicTrack anxiety = new MusicTrack("Anxiety");
    public MusicTrack curiosity = new MusicTrack("Curiosity");
    public MusicTrack battleIntensity = new MusicTrack("BattleIntensity");

    //Bind the two FMOD parameters to functions called "Anxiety" and "Curiousity"
    #region Anxiety Music
    public void SetAnxiety(float newLevel) //Sets the Anxiety level to one exact number
    {
        Instance.anxiety.SetLevel(musicEmitter, newLevel);
    }

    public void AdjustAnxiety(float level)
    {
        Instance.anxiety.AdjustLevel(musicEmitter, level);
    }
    #endregion

    #region Curiosity Music
    public void SetCuriosity(float newLevel)
    {
        Instance.curiosity.SetLevel(musicEmitter, newLevel);
    }

    public void AdjustCuriosity(float level)
    {
        Instance.curiosity.AdjustLevel(musicEmitter, level);
    }
    #endregion

    #region Battle Music
    public void SetBattle(float newLevel)
    {
        Instance.battleIntensity.SetLevel(musicEmitter, newLevel);
    }

    public void AdjustBattle(float level)
    {
        Instance.battleIntensity.AdjustLevel(musicEmitter, level);
    }
    #endregion

    private void OnDestroy()
    {
        if (Instance.musicEmitter == null)
            return;
        if (!Instance.musicEmitter.EventInstance.isValid())
            return;
        Instance.musicEmitter.EventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); //Let the track fade out instead of stopping immediately on destroy
    }
}