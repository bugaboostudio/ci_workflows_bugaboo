using Fusion;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

namespace Fusion.Samples.IndustriesComponents
{
    /**
     * 
     * Request Android authorization for audio
     * Note: a Fusion bridge component is also required
     * 
     **/

    public class MicrophoneAuthorization : MonoBehaviour
    {
        public bool waitingForMicrophonePermissionAnwer = false;
        public bool microphoneAccessGranted = false;
        [Tooltip("Automatically search on gameobject if not set")]
        public VoiceConnection voiceConnection;
        [Tooltip("Automatically search on gameobject, or created, if not set")]
        public Recorder recorder;

        [Header("Automatic callbacks")]
        public bool requestAuthorizationOnStart = true;
        public bool configureVoiceOnRequestAnswered = true;
        public List<MonoBehaviour> enableOnRequestAnswered = new List<MonoBehaviour>();
        public UnityEvent onRequestAnswered;

        private async void Start()
        {
            if (requestAuthorizationOnStart) await ActivateAudioCapture();
        }

        protected async virtual Task ActivateAudioCapture()
        {
            await RequestMicrophoneAccess();
            if (configureVoiceOnRequestAnswered)
            {
                if (voiceConnection == null) voiceConnection = GetComponent<VoiceConnection>();
                if (voiceConnection)
                {
                    if (recorder == null) recorder = GetComponentInChildren<Recorder>(true);
                    if (recorder == null)
                    {
                        recorder = gameObject.AddComponent<Recorder>();
                    }
                    recorder.gameObject.SetActive(true);
                    recorder.enabled = true;
                    voiceConnection.PrimaryRecorder = recorder;
                }
            }
            foreach (var behaviour in enableOnRequestAnswered)
            {
                behaviour.enabled = true;
            }
            if (onRequestAnswered != null) onRequestAnswered.Invoke();
        }

        protected async virtual Task RequestMicrophoneAccess()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            Debug.Log("[Microphone] Validating access");
            bool micAccess = Permission.HasUserAuthorizedPermission(Permission.Microphone);
            if (!micAccess)
            {
                var callbacks = new PermissionCallbacks();
                callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
                callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
                callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
                waitingForMicrophonePermissionAnwer = true;
                Permission.RequestUserPermission(Permission.Microphone, callbacks);
                while (waitingForMicrophonePermissionAnwer)
                {
                    await Task.Delay(10);
                }
            }

#else
        Debug.Log("[Microphone] Access granted (automatically)");
        microphoneAccessGranted = true;
        await Task.FromResult(0);
#endif
        }

        #region PermissionCallbacks
        protected virtual void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
        {
            Debug.LogError($"[Microphone] {permissionName} PermissionDeniedAndDontAskAgain");
            waitingForMicrophonePermissionAnwer = false;
        }

        protected virtual void PermissionCallbacks_PermissionGranted(string permissionName)
        {
            Debug.Log($"[Microphone] {permissionName} PermissionCallbacks_PermissionGranted");
            waitingForMicrophonePermissionAnwer = false;
            microphoneAccessGranted = true;
        }

        protected virtual void PermissionCallbacks_PermissionDenied(string permissionName)
        {
            Debug.LogError($"[Microphone] {permissionName} PermissionCallbacks_PermissionDenied");
            waitingForMicrophonePermissionAnwer = false;
        }
        #endregion

    }

}

