using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RunicSounds;

public class FMODTest : MonoBehaviour
{
    [SerializeField] private AudioBank audioBank = new AudioBank();

    [SerializeField] private AudioEvent audioEventWave = new AudioEvent();

    [SerializeField] private AudioParameter audioParameterPitch = new AudioParameter();

    [SerializeField] private AudioEvent audioEventMusic = new AudioEvent();

    [SerializeField] private List<AudioEvent> audioEventList = new List<AudioEvent>();
    [SerializeField] private List<AudioParameter> audioParamList = new List<AudioParameter>();
    [SerializeField] private List<AudioBank> audioBankList = new List<AudioBank>();

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        audioBank.LoadBank();
        while (audioBank.IsLoadOperationInProgress) { yield return null; }
        Debug.Log("Has bank loaded: " + audioBank.IsLoaded);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1)) {
        //    audioEventWave.PlayOneShot(gameObject, null);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2)) {
        //    audioEventWave.PlayOneShot(gameObject, audioParameterPitch.Set(0));
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha3)) {
        //    audioEventWave.PlayOneShot(gameObject, audioParameterPitch.Set(1));
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha4)) {
        //    audioEventMusic.Play();
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha5)) {
        //    audioEventMusic.Stop();
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha6)) {
        //    audioEventList[0].PlayOneShot(gameObject);
        //}
    }
}
