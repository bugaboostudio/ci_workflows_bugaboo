using System;
using UnityEngine;
using Fusion.XR;

/**
 * 
 * The AmbiantAudioManager allows you to configure the various ambiant sounds
 * 
 **/

namespace Fusion.Samples.IndustriesComponents
{
    public class AmbiantSoundManager : MonoBehaviour
    {

        public String[] ambiantSounds;
        public SoundManager soundmanager;


        void Start()
        {
            if (soundmanager == null)
            {
                Debug.LogError("SoundManager not referenced");
            }

            foreach (string ambiantSound in ambiantSounds)
            {
                for (int i = 0; i < soundmanager.sounds.Count; i++)
                {
                    if (soundmanager.sounds[i].name == ambiantSound)
                    {
                        Sound sound = soundmanager.sounds[i];

                        AudioSource audioSource = gameObject.AddComponent<AudioSource>();

                        audioSource.clip = sound.clip;
                        audioSource.volume = sound.volume;
                        audioSource.pitch = sound.pitch;
                        audioSource.loop = sound.loop;
                        audioSource.spatialBlend = sound.spatialBlend;
                        audioSource.outputAudioMixerGroup = sound.outputAudioMixerGroup;
                        audioSource.playOnAwake = true;

                        soundmanager.PlayRandomPosition(ambiantSound, audioSource);
                        break;
                    }
                }
            }
        }
    }
}
