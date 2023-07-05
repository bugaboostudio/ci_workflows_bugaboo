using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Voice.Unity;
using Fusion.XR;
using Fusion.XR.Shared.Interaction;

/**
 *
 * AudioSettingsMenu restore volume sliders with the audio setting manager values.
 * A listener is created for each slider in order to call the audio manager and save the new value.
 * It also create a button for each microphone. The microphone button state is updated when it is selected by the user and then, saved in preference settings.
 * 
 **/

namespace Fusion.Samples.IndustriesComponents
{
    public class AudioSettingsMenu : MonoBehaviour
    {
        public Slider masterVolume;
        public Slider voiceVolume;
        public Slider ambienceVolume;
        public Slider effectVolume;

        public RectTransform microphoneParent;
        public GameObject buttonPrefab;
        public GameObject labelPrefab;

        public VoiceConnection voiceConnection;
        public Recorder recorder;
        public AudioSettingsManager audioSettingsManager;

        public AudioSource audioSource;

        public MenuManager menuManager;


        private void OnEnable()
        {
            if (menuManager == null) menuManager = GetComponentInParent<MenuManager>();
            if (voiceConnection == null) voiceConnection = menuManager.managers.voiceConnection;
            if (recorder == null) recorder = voiceConnection.PrimaryRecorder;
            if (recorder == null)
            {
                return;
            }

            if (audioSettingsManager == null) audioSettingsManager = menuManager.managers.audioSettingsManager;
            if (audioSettingsManager == null)
            {
                Debug.LogError("Audio Settings Manager not found");
                return;
            }

            if (audioSource == null)
            {
                Debug.LogError("AudioSource not found");
                return;
            }


            masterVolume.onValueChanged.RemoveListener(audioSettingsManager.SetMasterVolume);
            voiceVolume.onValueChanged.RemoveListener(audioSettingsManager.SetVoiceVolume);
            ambienceVolume.onValueChanged.RemoveListener(audioSettingsManager.SetAmbienceVolume);
            effectVolume.onValueChanged.RemoveListener(audioSettingsManager.SetEffectVolume);

            // restore saved volume parameters
            masterVolume.value = audioSettingsManager.masterVolume;
            voiceVolume.value = audioSettingsManager.voiceVolume;
            ambienceVolume.value = audioSettingsManager.ambienceVolume;
            effectVolume.value = audioSettingsManager.effectVolume;

            // Add listeners on volume sliders
            masterVolume.onValueChanged.AddListener(audioSettingsManager.SetMasterVolume);
            voiceVolume.onValueChanged.AddListener(audioSettingsManager.SetVoiceVolume);
            ambienceVolume.onValueChanged.AddListener(audioSettingsManager.SetAmbienceVolume);
            effectVolume.onValueChanged.AddListener(audioSettingsManager.SetEffectVolume);

            CreateMicrophoneButtons();

        }


        // create microphones buttons
        public void CreateMicrophoneButtons()
        {
            recorder.MicrophonesEnumerator.Refresh();
            DestroyAllChildren(microphoneParent);

#if (UNITY_ANDROID || UNITY_WEBGL) && !UNITY_EDITOR
            return;
#endif

            List<string> options = new List<string>();
            int selectedIndex = -1;
            var devices = recorder.MicrophonesEnumerator;

            if (devices.IsSupported)
            {

#if !UNITY_WEBGL
                // Search for the microphone index
                if (recorder.MicrophoneType == Recorder.MicType.Unity)
                {
                    // Unity microphone
                    options = new List<string>(Microphone.devices);
                    selectedIndex = options.FindIndex(item => Recorder.CompareUnityMicNames(item, recorder.UnityMicrophoneDevice));
                }
                else
                {   // Photon microphone
                    int i = 0;
                    foreach (var device in devices)
                    {
                        options.Add(device.Name);
                        if (device.IDInt == recorder.PhotonMicrophoneDeviceId)
                        {
                            selectedIndex = i;
                        }
                        i++;
                    }
                }
#endif

                // Instantiate microphone buttons
                for (int i = 0; i < options.Count; ++i)
                {
                    GameObject go = null;
                    if (i == selectedIndex)
                    {
                        go = Instantiate(labelPrefab, microphoneParent);
                    }
                    else
                    {
                        int index = i;
                        go = Instantiate(buttonPrefab, microphoneParent);
                        var button = go.GetComponent<Button>();

                        TouchableCanvas touchableCanvas = GetComponentInParent<TouchableCanvas>();
                        if (touchableCanvas && touchableCanvas.touchableButtonExtensionPrefab)
                        {
                            var touchable = GameObject.Instantiate(touchableCanvas.touchableButtonExtensionPrefab, button.transform);
                            touchable.transform.position = button.transform.position;
                            touchable.transform.rotation = button.transform.rotation;
                            touchable.transform.localScale = button.transform.localScale;
                        }

                        if (button != null)
                        {
                            button.onClick.AddListener(delegate ()
                            {
                                OnInputDeviceChanged(index);
                            });
                        }
                    }

                    var label = go.GetComponentInChildren<TextMeshProUGUI>();
                    label.text = options[i];

                    var audiofeedback = go.GetComponent<UIAudioFeedback>();
                    if (audiofeedback)
                    {
                        audiofeedback.soundManager = menuManager.managers.soundManager;
                        audiofeedback.audioSource = audioSource;
                    }
                }
            }
        }

        // Destroy all space holder microphones
        public void DestroyAllChildren(Transform parent)
        {
            if (parent == null)
            {
                return;
            }

            int childCount = parent.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                GameObject.Destroy(parent.GetChild(i).gameObject);
            }
        }

        // Update the microphone selected by the user and save it in preference settings
        void OnInputDeviceChanged(int value)
        {
#if !UNITY_WEBGL
            if (recorder.MicrophoneType == Recorder.MicType.Unity)
            {
                try
                {
                    recorder.UnityMicrophoneDevice = Microphone.devices[value];
                    PlayerPrefs.SetString("UnityMic", recorder.UnityMicrophoneDevice);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
            }
            else
            {
                try
                {
                    recorder.PhotonMicrophoneDeviceId = recorder.MicrophonesEnumerator.IDAtIndex(value);
                    PlayerPrefs.SetInt("PhotonMic", recorder.PhotonMicrophoneDeviceId);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
            }

            if (recorder.RequiresRestart)
            {
                recorder.RestartRecording();
            }

            CreateMicrophoneButtons();
#endif
        }
    }
}