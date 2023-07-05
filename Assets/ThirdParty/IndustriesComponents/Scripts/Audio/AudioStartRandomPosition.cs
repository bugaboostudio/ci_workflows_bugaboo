using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 * The AudioStartRandomPosition search for an AudioSource on the gameObject and start playing the audioclip at a random start position
 * 
 **/

namespace Fusion.Samples.IndustriesComponents
{
    public partial class AudioStartRandomPosition : MonoBehaviour
    {
        public AudioClip audioClip;
        private AudioSource audioSource;
        void Start()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.clip = audioClip;
            int randomStartTime = Random.Range(0, audioClip.samples - 1);
            audioSource.timeSamples = randomStartTime;
            audioSource.Play();
        }
    }
}
