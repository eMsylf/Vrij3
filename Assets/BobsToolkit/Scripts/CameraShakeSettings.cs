using System;
using UnityEngine;

namespace BobJeltes.StandardUtilities
{
    [Serializable]
    public class CameraShakeSettings
    {
        public bool enabled = true;
        [Min(0.001f)]
        public float Duration = .5f;
        [Min(0.001f)]
        public float Strength = .6f;
    }
}