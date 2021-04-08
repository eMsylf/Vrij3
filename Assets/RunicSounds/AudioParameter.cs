using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using RunicSounds.EngineWrapper;
using FMODUnity;

namespace RunicSounds {

    [System.Serializable]
    public class AudioParameter {
        [SerializeField] private PersistentFMODParamField persistentFMODParamField = new PersistentFMODParamField();

        /// <summary>
        /// Use to set parameter.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="context"></param>
        public ActiveAudioParameter Set(float value, bool instant = false) {
            return new ActiveAudioParameter(persistentFMODParamField.ID, value, instant);
            //AudioEngine.SetParameter(persistentFMODParamField.ID, value, context);
        }

        public void SetGlobal(float value, bool instant = false) {
            RuntimeManager.StudioSystem.setParameterByID(persistentFMODParamField.ID, value, instant);
        }
    }

    public class ActiveAudioParameter {
        public readonly PARAMETER_ID ID;
        public readonly float Value;
        public readonly bool Instant;

        public ActiveAudioParameter(PARAMETER_ID id, float value, bool isInstant = false) {
            this.ID = id;
            this.Value = value;
            this.Instant = isInstant;
        }
    }

}
