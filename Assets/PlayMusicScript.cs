using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusicScript : MonoBehaviour
{
    private static FMOD.Studio.EventInstance Music;

    void Start()
    {
        Music = FMODUnity.RuntimeManager.CreateInstance("event:/DUNGEON1");
        Music.start(); //If the instance was already playing then calling this function will restart the event.
        Music.release(); //This function marks the event instance to be released. Event instances marked for release are destroyed by the asynchronous update when they are in the stopped state (FMOD_STUDIO_PLAYBACK_STOPPED).

    }

    //Bind the two FMOD parameters to functions called "Anxiety" and "Curiousity"
    public void Anxiety(int anxietyLevel)
    {
        Music.setParameterByName("Anxiety", anxietyLevel);
    }

    public void Curiousity(int curiousityLevel)
    {
        Music.setParameterByName("Curiousity", curiousityLevel);
    }

    private void OnDestroy()
    {
        Music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); //Let the track fade out instead of stopping immediately on destroy
    }
}