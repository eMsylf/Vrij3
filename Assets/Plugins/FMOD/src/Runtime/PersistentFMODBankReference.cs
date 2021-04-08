using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunicSounds {
    namespace EngineWrapper {

        [System.Serializable]
        public class PersistentFMODBankReference : ScriptableObject {

            public Guid GUID {
                get {
                    return new Guid(fmodGUID);
                }
            }

            public string Name {
                get {
                    return stringFMODRef;
                }
            }

            [SerializeField] private string stringFMODRef = string.Empty;
            [SerializeField] private byte[] fmodGUID = new byte[16];

#if UNITY_EDITOR
            public bool Exists;
#endif
        }
    }
}
