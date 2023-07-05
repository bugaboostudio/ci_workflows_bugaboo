using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/**
 * 
 * Set the initial audio interest group
 * 
 **/

namespace Fusion.XR
{
    public class InitialAudioInterestGroup : MonoBehaviour
    {
        public VoiceConnection voiceConnection;
        public Recorder recorder;
        public byte initialInterestGroup = 0;
        public bool listenToOwnGroup = true;


        async private void OnEnable()
        {
            while (!recorder.IsRecording) await Task.Delay(10);
            Debug.Log("[Photon Voice] Switch Recorder to default interest group: " + initialInterestGroup);
            if (listenToOwnGroup)
            {
                voiceConnection.Client.OpChangeGroups(groupsToRemove: new byte[] { }, groupsToAdd: new byte[] { initialInterestGroup });
            }
            recorder.InterestGroup = initialInterestGroup;
        }
    }

}