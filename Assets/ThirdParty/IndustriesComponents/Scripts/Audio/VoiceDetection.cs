using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 * The VoiceDetection OnAudioFilterRead compute an average voiceVolume for data received. It is used to animate avatar mouth if voice volume exceed a specific threshold.
 * 
 **/

namespace Fusion.Samples.IndustriesComponents
{
    public class VoiceDetection : MonoBehaviour
    {
        public AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }


        private void Update()
        {
            // reset the voice volume to stop lip sync when user exit a chat bubble
            if (audioSource && !audioSource.enabled)
                voiceVolume = 0;
        }

        public float voiceVolume = 0;
        private void OnAudioFilterRead(float[] data, int channels)
        {
            voiceVolume = 0f;
            foreach (var sample in data)
            {
                voiceVolume += Mathf.Abs(sample);
            }
            voiceVolume /= data.Length;
        }
    }
}
