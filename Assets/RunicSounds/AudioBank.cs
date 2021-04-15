using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RunicSounds.EngineWrapper;
using FMODUnity;

namespace RunicSounds {

    [System.Serializable]
    public class AudioBank {
        [SerializeField] private PersistentFMODBankReference bankReference = null;

        public bool IsLoaded {
            get {
                return RuntimeManager.HasBankLoaded(bankReference.Name);
            }
        }

        public bool IsLoadOperationInProgress {
            get {
                return RuntimeManager.AnyBankLoading();
            }
        }

        /// <summary>
        /// Will start async loading of bank and related sampledata. Use IsLoadOperationInProgress to check if load operation is completed.
        /// </summary>
        public void LoadBank() {
            RuntimeManager.LoadBank(bankReference.Name, true);
        }

        /// <summary>
        /// Will start async unloading of bank and related sampledata.
        /// </summary>
        public void UnloadBank() {
            RuntimeManager.UnloadBank(bankReference.Name);
        }
    }
}
