using UnityEngine.Audio;
using UnityEngine;

/**
 * 
 * The Sound class encompasses the various sounds parameters.
 * 
 **/

namespace Fusion.XR
{
    [System.Serializable]
    public class Sound
    {

        public string name;
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume = 0.5f;
        [Range(.5f, 1.5f)]
        public float pitch = 1f;

        public bool loop;

        [Range(0f, 1f)]
        public float spatialBlend;

        public AudioMixerGroup outputAudioMixerGroup;

    }
}
