using Fusion.XR.Shared.Rig;
using Photon.Realtime;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.XR.Zone
{
    /**
     * If set next to a Zoneuser for the local user, when entering a zone, will change the voice interest group
     */
    [RequireComponent(typeof(ZoneUser))]
    public class ZoneAudioInterestChanger : MonoBehaviour, IZoneUserListener
    {
        public ZoneUser zoneUser;
        public Recorder recorder;
        public VoiceConnection voiceConnection;
        InitialAudioInterestGroup initialAudioInterestGroup; 
        byte originalInterestGroup;
        public List<byte> mutedGroups = new List<byte> { };

        private AudioSource audioSource;
        RigInfo rigInfo;

        private void Awake()
        {
            zoneUser = GetComponent<ZoneUser>();
            zoneUser.RegisterListener(this);
            audioSource = GetComponentInChildren<AudioSource>();
            audioSource.enabled = false;
        }

        private void OnDestroy()
        {
            if(zoneUser) zoneUser.UnregisterListener(this);
        }

        void ConfigureAudio()
        {
            if(voiceConnection == null) voiceConnection = zoneUser.rig.Runner.GetComponentInChildren<VoiceConnection>();
            if(recorder == null) recorder = voiceConnection.PrimaryRecorder;
            if(initialAudioInterestGroup == null) initialAudioInterestGroup = voiceConnection.GetComponentInChildren<InitialAudioInterestGroup>();
            if (initialAudioInterestGroup && initialAudioInterestGroup.listenToOwnGroup == false)
            {
                // The starting channel is muted (users don't listen to it): add it to the muted list
                mutedGroups.Add(initialAudioInterestGroup.initialInterestGroup);
            }
        }

        bool debugAudioSourceManagement = false;

        /**
         * Unity can not handle too much activa audio sources at the same time.
         *  So we have to limit to the really useful audio sources, that is, the one associated to an audio zone where the local user is
         */
        protected virtual void ManageAudioSources(Zone previous, Zone current) {
            // 1 - if a zoneUser is not in a zone anymore, we desactivate its audioSource
            if (current == null)
            {
                if (debugAudioSourceManagement) Debug.LogError($"[{zoneUser.rig.Object.StateAuthority.ToString()}] 1 - if a zoneUser is not in a zone anymore, we desactivate its audioSource");
                audioSource.enabled = false;
            }

            if(rigInfo == null && zoneUser.rig.Runner != null)
            {
                rigInfo = RigInfo.FindRigInfo(zoneUser.rig.Runner);
            }
            // 2 - if a ZoneUser enters a zone where we are, we activate its audioSource
            if(current != null && current.RigsInzone.Contains(rigInfo.localNetworkedRig))
            {
                if (debugAudioSourceManagement) Debug.LogError($"[{zoneUser.rig.Object.StateAuthority.ToString()}] 2 - if a ZoneUser enters a zone where we are, we activate its audioSource");
                audioSource.enabled = true;
            }

            // 3 - if the local Zoneuser enters a Zone, every ZoneUser in this zone must see its audioSource activated
            if (current != null && zoneUser.rig.IsLocalNetworkRig)
            {
                foreach(var rigAlreadyInZoneId in current.RigsInzone)
                {
                    if (current.Runner.TryFindBehaviour(rigAlreadyInZoneId, out NetworkBehaviour rigAlreadyInZone))
                    {
                        ZoneAudioInterestChanger rigAlreadyInZoneAudioInterestChanger = rigAlreadyInZone.GetComponentInChildren<ZoneAudioInterestChanger>();
                        if (rigAlreadyInZoneAudioInterestChanger)
                        {
                            if (debugAudioSourceManagement) Debug.LogError($"[{rigAlreadyInZoneAudioInterestChanger.zoneUser.rig.Object.StateAuthority.ToString()}] 3 - if the local Zoneuser enters a Zone, every ZoneUser in this zone must see its audioSource activated");
                            rigAlreadyInZoneAudioInterestChanger.audioSource.enabled = true;
                        }
                    }
                }
            }

            // 4 - if the local user is exiting a (non global) zone, every ZoneUser in this zone must see its audioSource desactivated
            if (current == null && previous != null && zoneUser.rig.IsLocalNetworkRig &&!previous.IsGlobalAudioGroup)
            {
                foreach (var rigAlreadyInZoneId in previous.RigsInzone)
                {
                    if (previous.Runner.TryFindBehaviour(rigAlreadyInZoneId, out NetworkBehaviour rigAlreadyInZone))
                    {
                        ZoneAudioInterestChanger rigAlreadyInZoneAudioInterestChanger = rigAlreadyInZone.GetComponentInChildren<ZoneAudioInterestChanger>();
                        if (rigAlreadyInZoneAudioInterestChanger)
                        {
                            if (debugAudioSourceManagement) Debug.LogError($"[{rigAlreadyInZoneAudioInterestChanger.zoneUser.rig.Object.StateAuthority.ToString()}] 4 - if the local user is exiting a (non global) zone, every ZoneUser in this zone must see its audioSource desactivated");
                            rigAlreadyInZoneAudioInterestChanger.audioSource.enabled = false;
                        }
                    }
                }
            }

            // 5 - if the ZoneUser enters a global audio group, we enable the audio source and change sounds to broadcast
            if (current != null && current.IsGlobalAudioGroup)
            {
                if (debugAudioSourceManagement) Debug.LogError($"[{zoneUser.rig.Object.StateAuthority.ToString()}] 5 - if the ZoneUser enters a global audio group, we enable the audio source and change sounds to broadcast");
                audioSource.enabled = true;
                audioSource.spatialBlend = 0;
            }

            // 6 - If exiting a global audio group, we enable the audio source
            if(previous != null && previous.IsGlobalAudioGroup)
            {
                if (current == null || !current.IsGlobalAudioGroup)
                {
                    if (debugAudioSourceManagement) Debug.LogError($"[{zoneUser.rig.Object.StateAuthority.ToString()}] 6 - If exiting a global audio group, we enable the audio source");
                    audioSource.spatialBlend = 1;
                }
            }
        }

        #region IZoneUserListener
        public async void OnZoneChanged(Zone previous, Zone current)
        {
            ManageAudioSources(previous, current);

            // We check if it is the local user (and not a bot)
            if (zoneUser.nonUserRig != null || !zoneUser.rig.Object.HasInputAuthority) return;

            if (recorder == null) ConfigureAudio();

            // We wait for the voice connection to be established before changing interest group
            float startTime = Time.time;
            while(voiceConnection.Client.State != ClientState.Joined && (Time.time - startTime) < 10)
            {
                await System.Threading.Tasks.Task.Delay(100);
                Debug.Log($"[ZoneAudioInterestChanger] Waiting for audio connection ({voiceConnection.Client.State}) before changing  ...");
            }

            if (previous && previous.hasAudioInterestGroup)
            {
                ExitingInterestGroup(previous.PhotonVoiceInterestGroup);
            }
            if (current && current.hasAudioInterestGroup)
            {
                EnteringInterestGroup(current.PhotonVoiceInterestGroup);
            }
        }

        public void EnteringInterestGroup(byte interestGroup)
        {
            if (recorder.InterestGroup == interestGroup) return;
            originalInterestGroup = recorder.InterestGroup;
            // We say that we want to speak in the interestGroup
            recorder.InterestGroup = interestGroup;
            // We say that we want to listen to the interestGroup
            if (mutedGroups.Contains(interestGroup))
            {
                voiceConnection.Client.OpChangeGroups(groupsToRemove: new byte[] { originalInterestGroup }, groupsToAdd: null);
                activeInterestGroup = new byte[] { };
            }
            else
            {
                voiceConnection.Client.OpChangeGroups(groupsToRemove: new byte[] { originalInterestGroup }, groupsToAdd: new byte[] { interestGroup });
                activeInterestGroup = new byte[] { interestGroup };
            }
            Debug.Log($"[InterestGroup] Enter {originalInterestGroup} => {recorder.InterestGroup}");

        }

        public byte[] activeInterestGroup;

        public void ExitingInterestGroup(byte interestGroup)
        {
            // We say that we want to stop listening to the interestGroup
            if (mutedGroups.Contains(originalInterestGroup))
            {
                voiceConnection.Client.OpChangeGroups(groupsToRemove: new byte[] { interestGroup }, groupsToAdd: null);
                activeInterestGroup = new byte[] {  };
            }
            else
            {
                voiceConnection.Client.OpChangeGroups(groupsToRemove: new byte[] { interestGroup }, groupsToAdd: new byte[] { originalInterestGroup });
                activeInterestGroup = new byte[] { originalInterestGroup };
            }
            Debug.Log($"[InterestGroup] Exit {recorder.InterestGroup} => {originalInterestGroup}");
            if (recorder.InterestGroup != interestGroup) return;
            // If we were still speaking in this group (no rapid change to another gorup), say that we want to stop speaking in the interestGroup
            recorder.InterestGroup = originalInterestGroup;
        }
        #endregion

    }
}
