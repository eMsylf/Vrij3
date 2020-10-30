using System;
using UnityEngine;

namespace BobJeltes.StandardUtilities
{
    [Serializable]
    public class HitStunSettings
    {
        public bool enabled = true;
        [Min(0.001f)]
        public float Slowdown = .01f;
        [Min(0.001f)]
        public float Duration = .5f;
    }
}