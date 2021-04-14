using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System;
using FMOD.Studio;
using RunicSounds.EngineWrapper;

namespace RunicSounds {

    [System.Serializable]
    public class AudioEvent
    {
        public bool IsPlaying {
            get {
                return activeAudioEvent != null;
            }
        }

        public bool IsValid {
            get {
                return persistentFMODField.IsValid;
            }
        }

        [SerializeField] private PersistentFMODEventField persistentFMODField = new PersistentFMODEventField();
        private ActiveAudioEvent activeAudioEvent = null;

#if UNITY_EDITOR
        public void DebugFMODFieldValues() {
            Debug.Log("FMODFIELD VALUES: " + persistentFMODField.GUID);
        }
#endif

        /// <summary>
        /// Make sure Stop is called, otherwise memory issues might occur. Needs linked object for 3D information.
        /// </summary>
        /// <param name="linkedObject"></param>
        public void Play(GameObject linkedObject = null) {
            Stop();
            activeAudioEvent = new ActiveAudioEvent(persistentFMODField.GUID, linkedObject);
        }

        /// <summary>
        /// Make sure Stop is called, otherwise memory issues might occur. Needs position for 3D information.
        /// </summary>
        /// <param name="linkedObject"></param>
        public void Play(Vector3 position)
        {
            Stop();
            activeAudioEvent = new ActiveAudioEvent(persistentFMODField.GUID, position);
        }

        /// <summary>
        /// Cannot be stopped. Needs linked object for 3D information.
        /// </summary>
        /// <param name="linkedObject"></param>
        public void PlayOneShot(GameObject linkedObject, params ActiveAudioParameter[] parameters) {
            activeAudioEvent = new ActiveAudioEvent(persistentFMODField.GUID, linkedObject);
            SetParameters(parameters);
            activeAudioEvent.Release();
            activeAudioEvent = null;
        }

        /// <summary>
        /// Cannot be stopped. Needs Vector3 for 3D information.
        /// </summary>
        /// <param name="linkedObject"></param>
        public void PlayOneShot(Vector3 position, params ActiveAudioParameter[] parameters) {
            activeAudioEvent = new ActiveAudioEvent(persistentFMODField.GUID, position);
            SetParameters(parameters);
            activeAudioEvent.Release();
            activeAudioEvent = null;
        }

        /// <summary>
        /// Do not call before calling Play, parameter will not be applied.
        /// </summary>
        /// <param name="parameters"></param>
        public void SetParameters(params ActiveAudioParameter[] parameters) {
            if (activeAudioEvent == null) { return; }
            foreach (var parameter in parameters) {
                activeAudioEvent.SetParameter(parameter.ID, parameter.Value, parameter.Instant);
            }
        }

        /// <summary>
        /// Stop the audio event currently playing.
        /// </summary>
        public void Stop() {
            if (activeAudioEvent != null) {
                activeAudioEvent.Stop();
                activeAudioEvent = null;
            }
        }

        private class ActiveAudioEvent {
            public EventInstance FMODEventInstance;

            public ActiveAudioEvent(Guid Guid, GameObject LinkedObject) {
                this.FMODEventInstance = RuntimeManager.CreateInstance(Guid);
                if (LinkedObject != null) {
                    RuntimeManager.AttachInstanceToGameObject(this.FMODEventInstance, LinkedObject.transform, LinkedObject.GetComponent<Rigidbody>());
                }
                this.FMODEventInstance.start();
            }

            public ActiveAudioEvent(Guid Guid, Vector3 position) {
                this.FMODEventInstance = RuntimeManager.CreateInstance(Guid);
                this.FMODEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
                this.FMODEventInstance.start();
            }

            public void Stop() {
                this.FMODEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                Release();
            }

            public void Release() {
                this.FMODEventInstance.release();
            }

            public void SetParameter(PARAMETER_ID parameterID, float value, bool instant) {
                this.FMODEventInstance.setParameterByID(parameterID, value, instant);
            }
        }
    }
}
