using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 *  FadeAudioSource increase the audiosource volume from start to targetVolume for the duration period of time
 *  
 **/

namespace Fusion.XR
{
    public static class FadeAudioSource
    {
        public static IEnumerator StartFade(AudioSource audioSource, float start, float duration, float targetVolume)
        {
            float currentTime = 0;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
                yield return null;
            }
            yield break;
        }
    }
}