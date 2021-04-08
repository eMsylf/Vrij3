using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RunicSounds {
    public abstract class AudioVolume : MonoBehaviour {

        public abstract Vector3 GetClosestVolumePoint(Vector3 point);

    }
}
