using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BobJeltes.StandardUtilities;

public class PlayMusicScript : Singleton<PlayMusicScript>
{
    public FMODUnity.StudioEventEmitter musicEmitter;

    void Start()
    {
        if (Instance.musicEmitter == null) Debug.LogError("MusicEmitter is Missing");
        else Instance.musicEmitter.Play();
    }

    //Bind the two FMOD parameters to functions called "Anxiety" and "Curiousity"
    #region Anxiety Music

    public void SetAnxiety(float anxietyLevel) //Sets the Anxiety level to one exact number
    {
        Instance.musicEmitter.SetParameter("Anxiety", anxietyLevel);
    }

    public void IncreaseAnxiety(float anxietyLevel)
    {
        float currentAnxietyLevel;
        Instance.musicEmitter.EventInstance.getParameterByName("Anxiety", out currentAnxietyLevel );
        float newAnxietyLevel = currentAnxietyLevel + anxietyLevel;
        Instance.musicEmitter.SetParameter("Anxiety", newAnxietyLevel);
    }

    public void DecreaseAnxiety(float anxietyLevel) //Decrease the Anxiety level
    {
        float currentAnxietyLevel;
        Instance.musicEmitter.EventInstance.getParameterByName("Anxiety", out currentAnxietyLevel);
        float newAnxietyLevel = currentAnxietyLevel - anxietyLevel;
        Instance.musicEmitter.SetParameter("Anxiety", newAnxietyLevel);
    }
    #endregion

    #region Curiosity Music

    public void SetCuriousity(float curiousityLevel)
    {
        Instance.musicEmitter.SetParameter("Curiousity", curiousityLevel);
    }

    public void IncreaseCuriousity(float curiousityLevel)
    {
        float currentLevel;
        Instance.musicEmitter.EventInstance.getParameterByName("Curiousity", out currentLevel);
        float newLevel = currentLevel + curiousityLevel;
        Instance.musicEmitter.SetParameter("Curiousity", newLevel);
    }

    public void DecreaseCuriousity(float curiousityLevel) //Decrease the Anxiety level
    {
        float currentLevel;
        Instance.musicEmitter.EventInstance.getParameterByName("Curiousity", out currentLevel);
        float newLevel = currentLevel - curiousityLevel;
        Instance.musicEmitter.SetParameter("Curiousity", newLevel);

        Debug.Log(newLevel);
    }
    #endregion

    #region Battle Music

    public void SetBattle(float battleLevel) //Sets the Anxiety level to one exact number
    {
        Instance.musicEmitter.SetParameter("BattleIntensity", battleLevel);
    }

    public void IncreaseBattle(float battleLevel)
    {
        float currentLevel;
        Instance.musicEmitter.EventInstance.getParameterByName("BattleIntensity", out currentLevel);
        float newLevel = currentLevel + battleLevel;
        Instance.musicEmitter.SetParameter("BattleIntensity", newLevel);

        Debug.Log(newLevel);
    }

    public void DecreaseBattle(float battleLevel) //Decrease the Anxiety level
    {
        float currentLevel;
        Instance.musicEmitter.EventInstance.getParameterByName("BattleIntensity", out currentLevel);
        float newLevel = currentLevel - battleLevel;
        Instance.musicEmitter.SetParameter("BattleIntensity", newLevel);
        
        Debug.Log(newLevel);
    }
    #endregion

    private void OnDestroy()
    {
        if (Instance.musicEmitter == null)
            return;
        Instance.musicEmitter.EventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); //Let the track fade out instead of stopping immediately on destroy
    }
}