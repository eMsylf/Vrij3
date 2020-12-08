using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BobJeltes.StandardUtilities;

public class PlayMusicScriptLegacy : Singleton<PlayMusicScriptLegacy>
{
    public FMODUnity.StudioEventEmitter musicEmitter;

    public float AnxStart = 0f;
    public float BattleStart = 0f;
    public float CurStart = 0f;
    public bool useStartValues;
    public void ApplyStartValues()
    {
        Instance.Set(AnxietyName, AnxStart);
        Instance.Set(BattleName, BattleStart);
        Instance.Set(CuriosityName, CurStart);
    }

    void Start()
    {
        if (Instance.musicEmitter == null)
        {
            Debug.LogError("MusicEmitter is Missing");
            return;
        }
        
        Instance.musicEmitter.Play();

        if (useStartValues)
        {
            ApplyStartValues();
        }
    }

    private void Set(string parameterName, float level)
    {
        musicEmitter.SetParameter(parameterName, level);
        Debug.Log("Attempt to set " + parameterName + " level to " + level);
        musicEmitter.EventInstance.getParameterByName(parameterName, out float newValue);
        Debug.Log(parameterName + " level set to " + newValue);
    }

    //Bind the two FMOD parameters to functions called "Anxiety" and "Curiousity"
    #region Anxiety Music
    public string AnxietyName = "Anxiety";
    public void SetAnxiety(float anxietyLevel) //Sets the Anxiety level to one exact number
    {
        Instance.Set(AnxietyName, anxietyLevel);
    }

    public void AdjustAnxiety(float adjustment)
    {
        Instance.musicEmitter.EventInstance.getParameterByName(AnxietyName, out float currentAnxietyLevel);
        Instance.Set(AnxietyName, currentAnxietyLevel + adjustment);
    }
    #endregion

    #region Curiosity Music
    public string CuriosityName = "Curiosity";
    public void SetCuriousity(float curiousityLevel)
    {
        Instance.Set(CuriosityName, curiousityLevel);
    }

    public void AdjustCuriosity(float curiousityLevel)
    {
        Instance.musicEmitter.EventInstance.getParameterByName(CuriosityName, out float currentLevel);
        Instance.Set(CuriosityName, currentLevel + curiousityLevel);
    }
    #endregion

    #region Battle Music
    public string BattleName = "BattleIntensity";
    public void SetBattle(float battleLevel) //Sets the Anxiety level to one exact number
    {
        if (Instance == null) return;
        Instance.Set(BattleName, battleLevel);
    }

    public void AdjustBattle(float battleLevel)
    {
        if (Instance == null) return;
        Instance.musicEmitter.EventInstance.getParameterByName(BattleName, out float currentLevel);
        Instance.Set(BattleName, currentLevel + battleLevel);
    }
    #endregion

    private void OnDestroy()
    {
        if (Instance == null)
            return;
        if (Instance.musicEmitter == null)
            return;
        Instance.musicEmitter.EventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); //Let the track fade out instead of stopping immediately on destroy
    }
}