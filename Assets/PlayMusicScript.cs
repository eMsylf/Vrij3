using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BobJeltes.StandardUtilities;

public class PlayMusicScript : Singleton<PlayMusicScript>
{
    public FMODUnity.StudioEventEmitter musicEmitter;

    void Start()
    {
        if (musicEmitter == null) Debug.LogError("MusicEmitter is Missing");
        else musicEmitter.Play();
    }

    //Bind the two FMOD parameters to functions called "Anxiety" and "Curiousity"
    public void SetAnxiety(float anxietyLevel) //Sets the Anxiety level to one exact number
    {
        musicEmitter.SetParameter("Anxiety", anxietyLevel);
    }

    public void IncreaseAnxiety(float anxietyLevel)
    {
        float currentAnxietyLevel;
        musicEmitter.EventInstance.getParameterByName("Anxiety", out currentAnxietyLevel );
        float newAnxietyLevel = currentAnxietyLevel + anxietyLevel;
        musicEmitter.SetParameter("Anxiety", newAnxietyLevel);
    }

    public void DecreaseAnxiety(float anxietyLevel) //Decrease the Anxiety level
    {
        float currentAnxietyLevel;
        musicEmitter.EventInstance.getParameterByName("Anxiety", out currentAnxietyLevel);
        float newAnxietyLevel = currentAnxietyLevel - anxietyLevel;
        musicEmitter.SetParameter("Anxiety", newAnxietyLevel);
    }

    //Curriousity Level 

    public void SetCuriousity(float curiousityLevel)
    {
        musicEmitter.SetParameter("Curiousity", curiousityLevel);
    }

    public void IncreaseCuriousity(float curiousityLevel)
    {
        float currentLevel;
        musicEmitter.EventInstance.getParameterByName("Curiousity", out currentLevel);
        float newLevel = currentLevel + curiousityLevel;
        musicEmitter.SetParameter("Curiousity", newLevel);
    }

    public void DecreaseCuriousity(float curiousityLevel) //Decrease the Anxiety level
    {
        float currentLevel;
        musicEmitter.EventInstance.getParameterByName("Curiousity", out currentLevel);
        float newLevel = currentLevel - curiousityLevel;
        musicEmitter.SetParameter("Curiousity", newLevel);

        //Debug.Log("CuriousityLevel: "curiousityLevel);
    }

    //Battle Music

    public void SetBattle1(float battleLevel) //Sets the Anxiety level to one exact number
    {
        musicEmitter.SetParameter("BattleIntensity", battleLevel);
    }

    public void IncreaseBattle(float battleLevel)
    {
        float currentLevel;
        musicEmitter.EventInstance.getParameterByName("BattleIntensity", out currentLevel);
        float newLevel = currentLevel + battleLevel;
        musicEmitter.SetParameter("BattleIntensity", newLevel);

        Debug.Log(battleLevel);
    }

    public void DecreaseBattle(float battleLevel) //Decrease the Anxiety level
    {
        float currentLevel;
        musicEmitter.EventInstance.getParameterByName("BattleIntensity", out currentLevel);
        float newLevel = currentLevel - battleLevel;
        musicEmitter.SetParameter("BattleIntensity", newLevel);
    }

    private void OnDestroy()
    {
        if (musicEmitter == null)
            return;
        musicEmitter.EventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); //Let the track fade out instead of stopping immediately on destroy
    }
}