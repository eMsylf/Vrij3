using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;


namespace RunicSounds {
    namespace EngineWrapper {

        [System.Serializable]
        public class PersistentFMODEventField {

            public bool IsValid {
                get {
                    for (int i = 0; i < fmodGUID.Length; i++)
                    {
                        if (fmodGUID[i] != 0)
                        {
                            return true;
                        }
                    }
                    return false;
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
