using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;


namespace RunicSounds {
    namespace EngineWrapper {

        [System.Serializable]
        public class PersistentFMODEventField {
            private static readonly byte[] EMPTY_GUID = new byte[16] {
                0,0,0,0,
                0,0,0,0,
                0,0,0,0,
                0,0,0,0 };

            public bool IsValid {
                get {
                    return fmodGUID != EMPTY_GUID;
                }
            }

            public Guid GUID {
                get {
                    return new Guid(fmodGUID);
                }
            }
#if UNITY_EDITOR
            [SerializeField] private string stringFMODRef = string.Empty;
#endif
            [SerializeField] private byte[] fmodGUID = new byte[16];
        }
    }
}
