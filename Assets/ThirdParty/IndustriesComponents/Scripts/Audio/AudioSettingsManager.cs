using UnityEngine;
using Photon.Voice.Unity;
using UnityEngine.Audio;
using System.Linq;
using Fusion;

/**
 * 
 * AudioSettingsManager provides all functions to set the differents volumes (voice, ambience, effect) and save them user's preference settings
 * It is also in charge to restore volume settings & microphone saved in user's preference at start & boost mixer volume for android devices
 * 
 **/

namespace Fusion.Samples.IndustriesComponents
{
    public class AudioSettingsManager : MonoBehaviour
    {
        public AudioMixer mixer;
        public AnimationCurve volumeCurve;
        public VoiceConnection voiceConnection;
        public Recorder recorder;
        public Managers managers;

        public float androidMasterVolume = 10f;
        public float voiceVolume { get; private set; }
        public float masterVolume { get; private set; }
        public float ambienceVolume { get; private set; }
        public float effectVolume { get; private set; }



        // Boost mixer master volume for Android device
        public void InitializeMasterVolume()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
         mixer.SetFloat("Master", androidMasterVolume);
#endif
        }

        // Set the master volume and save the value
        public void SetMasterVolume(float value)
        {
            SetMasterVolume(value, true);
        }

        // Set the master volume and save the value into player settings according to the store parameter
        public void SetMasterVolume(float value, bool store = true)
        {
            masterVolume = value;
            AudioListener.volume = value;
            if (store == true)
            {
                PlayerPrefs.SetFloat("VolumeMaster", value);
            }
        }

        // Set the voice volume and save the value
        public void SetVoiceVolume(float value)
        {
            SetVoiceVolume(value, true);
        }

        // Set the voice volume and save the value into player settings according to the store parameter
        public void SetVoiceVolume(float value, bool store = true)
        {
            voiceVolume = value;
            SetVolume(value, store, "Voice Volume", "VolumeVoice");
        }

        // Set the ambience volume and save the value
        public void SetAmbienceVolume(float value)
        {
            SetAmbienceVolume(value, true);
        }

        // Set the ambience volume and save the value into player settings according to the store parameter
        public void SetAmbienceVolume(float value, bool store = true)
        {
            ambienceVolume = value;
            SetVolume(value, store, "Ambience Volume", "VolumeAmbience");
        }

        // Set the effect volume and save the value
        public void SetEffectVolume(float value)
        {
            SetEffectVolume(value, true);
        }

        // Set the effect volume and save the value into player settings according to the store parameter
        public void SetEffectVolume(float value, bool store = true)
        {
            effectVolume = value;
            SetVolume(value, store, "Effects Volume", "VolumeEffect");
        }

        // Set the mixer volume parameter and save the value into player settings according to the store parameter
        void SetVolume(float value, bool store, string mixerValue, string playerPrefsValue)
        {
            mixer.SetFloat(mixerValue, volumeCurve.Evaluate(value));
            if (store == true)
            {
                PlayerPrefs.SetFloat(playerPrefsValue, value);
            }
        }

        private void OnEnable()
        {
            if (managers == null) managers = Managers.FindInstance();
            if (voiceConnection == null) voiceConnection = managers.voiceConnection;
            if (recorder == null) recorder = voiceConnection.PrimaryRecorder;
            InitializeMasterVolume();
        }

        private void Start()
        {
            // restore previous volume settings
            SetMasterVolume(PlayerPrefs.GetFloat("VolumeMaster", 1f), false);
            SetVoiceVolume(PlayerPrefs.GetFloat("VolumeVoice", 1f), false);
            SetAmbienceVolume(PlayerPrefs.GetFloat("VolumeAmbience", 0.5f), false);
            SetEffectVolume(PlayerPrefs.GetFloat("VolumeEffect", 0.75f), false);

            // Do not restore microphone if it has been disabled
            if (PlayerPrefs.GetString("UnityMic") == "Disabled")
            {
                return;
            }

#if !UNITY_WEBGL
            if (recorder != null)
            {
                // refresh the microphones list
                recorder.MicrophonesEnumerator.Refresh();

                // try to restore the previous Unity microphone
                try
                {
                    string previousMicrophone = PlayerPrefs.GetString("UnityMic");
                    if (string.IsNullOrEmpty(previousMicrophone) == false && UnityMicrophone.devices.Contains(previousMicrophone))
                    {
                        recorder.UnityMicrophoneDevice = previousMicrophone;         // set UNITY mic by string
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }

                // try to restore the previous Photon microphone
                try
                {
                    recorder.PhotonMicrophoneDeviceId = PlayerPrefs.GetInt("PhotonMic", -1);    // set PHOTON mic by ID. internally makes sure the microphone can be used or uses default (-1).
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
            }
#endif
        }
    }
}
