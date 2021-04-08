using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

namespace RunicSounds {
    namespace EngineWrapper {

    [System.Serializable]
    public class PersistentFMODParamField {
        public PARAMETER_ID ID {
            get {
                PARAMETER_ID id = new PARAMETER_ID();
                id.data1 = paramdata1;
                id.data2 = paramdata2;
                return id;
            }
        }

#if UNITY_EDITOR
        [SerializeField] private string stringFMODRef = string.Empty;
#endif
        [SerializeField] private uint paramdata1 = 0;
        [SerializeField] private uint paramdata2 = 0;
        }

    }

}
