using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusicScript : MonoBehaviour
{
    public static PlayMusicScript _instance;
    public static PlayMusicScript Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<PlayMusicScript>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("MusicManager");
                    _instance = container.AddComponent<PlayMusicScript>();
                }
            }

            return _instance;
        }
    }

    public FMODUnity.StudioEventEmitter musicEmitter;

    void Start()
    {
        musicEmitter.Play();
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
        musicEmitter.EventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); //Let the track fade out instead of stopping immediately on destroy
    }
}