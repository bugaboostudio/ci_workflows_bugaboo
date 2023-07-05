using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Text;
using Photon.Voice.Unity;
using Fusion.XR;

namespace Fusion.Samples.IndustriesComponents
{

    public class DebugPanel : MonoBehaviour
    {
        public static string DisconnectCauseFusion = "";
        public static string DisconnectCauseVoice = "";

        public TextMeshProUGUI FusionConnectionText;
        public TextMeshProUGUI VoiceConnectionText;
        public TextMeshProUGUI LogText;

        public LogCollector logCollector;

        VoiceConnection voiceConnection;
        Recorder recorder;
        InitialAudioInterestGroup initialAudioInterestGroup;

        bool updateRequired = false;

        public MenuManager menuManager;

        private void OnEnable()
        {
            if (menuManager == null) menuManager = GetComponentInParent<MenuManager>();
            if (logCollector == null) logCollector = GetComponentInParent<LogCollector>();
            logCollector.onNewLogs.AddListener(HandleLogs);
            UpdateLogs();
        }

        private void OnDisable()
        {
            logCollector.onNewLogs.RemoveListener(HandleLogs);
        }

        private void Update()
        {
            if (updateRequired)
            {
                updateRequired = false;
                UpdateLogs();
            }
            UpdateFusion();
            UpdateVoice();
        }

        void HandleLogs()
        {
            updateRequired = true;
        }

        void UpdateFusion()
        {
            FusionConnectionText.text = $"{menuManager.managers.runner.State}";
        }

        void UpdateVoice()
        {
            if (voiceConnection == null) voiceConnection = menuManager.managers.voiceConnection;
            if (recorder == null) recorder = voiceConnection.PrimaryRecorder;
            if (initialAudioInterestGroup == null) initialAudioInterestGroup = voiceConnection.GetComponentInChildren<InitialAudioInterestGroup>();
            string voiceText = $"{voiceConnection.ClientState}";
            if (initialAudioInterestGroup.initialInterestGroup != recorder.InterestGroup || initialAudioInterestGroup.listenToOwnGroup)
            {
                voiceText += $" (Audio interest group: {recorder.InterestGroup})";
            }
            VoiceConnectionText.text = voiceText;
        }

        void UpdateLogs()
        {
            var builder = new StringBuilder();
            foreach (var log in logCollector.queue)
            {
                string text = log.condition.Substring(0, Math.Min(log.condition.Length, 120));
                if (log.type == UnityEngine.LogType.Error)
                {
                    text = "<color=#FF3F79>" + text + "</color>";
                }
                if (log.type == UnityEngine.LogType.Warning)
                {
                    text = "<color=\"orange\">" + text + "</color>";
                }

                builder.Append(text).Append("\n");
            }
            LogText.text = builder.ToString();
        }
    }


}
