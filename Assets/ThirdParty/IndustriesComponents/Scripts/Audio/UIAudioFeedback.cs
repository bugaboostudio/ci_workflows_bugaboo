using Fusion.XR;
using UnityEngine;
using UnityEngine.UI;


/**
 * 
 * Add audio feedback if user interacts with UI elements (button, slider, toggle)
 * 
 **/

namespace Fusion.Samples.IndustriesComponents
{
    public class UIAudioFeedback : MonoBehaviour
    {
        public SoundManager soundManager;
        public AudioSource audioSource;
        public string Type = "OnMenuItemSelected";

        void Start()
        {
            if (soundManager == null)
            {
                soundManager = SoundManager.FindInstance();
            }

            if (audioSource == null) audioSource = GetComponent<AudioSource>();

            var button = GetComponent<Button>();

            if (button != null)
            {
                button.onClick.AddListener(delegate ()
                {
                    PlaySound();
                });
            }

            var slider = GetComponent<Slider>();

            if (slider != null)
            {
                slider.onValueChanged.AddListener(delegate
                {
                    if (!audioSource.isPlaying)
                        PlaySound();
                });
            }

            var toggle = GetComponent<Toggle>();

            if (toggle != null)
            {
                toggle.onValueChanged.AddListener(delegate (bool newValue)
                {
                    if (newValue == true)
                    {
                        PlaySound();
                    }
                });
            }
        }

        private void PlaySound()
        {
            if (audioSource)
                soundManager.PlayOneShot(Type, audioSource);
            else
                soundManager.PlayOneShot(Type);
        }
    }
}
