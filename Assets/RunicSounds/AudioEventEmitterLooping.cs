using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RunicSounds;

public class AudioEventEmitterLooping : MonoBehaviour
{
    [SerializeField] private AudioEvent audioEvent = default;

    private void Start() {
        audioEvent.Play(gameObject, null);
    }

    private void OnDestroy() {
        audioEvent.Stop();
    }
}
