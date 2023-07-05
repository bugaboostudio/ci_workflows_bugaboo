using Fusion;
using Fusion.XR;
using Fusion.XR.Zone;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 * Reference all the application managers, to easily find them
 * Should be stored aside or under the NetworkRunner
 * 
 **/

namespace Fusion.Samples.IndustriesComponents
{
    public class Managers : MonoBehaviour
    {
        public ZoneManager zoneManager;
        public DynamicZonesPool dynamicZonespool;
        public AudioSettingsManager audioSettingsManager;
        public SoundManager soundManager;
        public VoiceConnection voiceConnection;
        public NetworkRunner runner;

        private void Awake()
        {
            if (runner == null) runner = GetComponentInParent<NetworkRunner>();
        }

        public static Managers FindInstance(NetworkRunner runner = null)
        {
            Managers managers = null;
            if (runner == null)
            {
                Debug.LogWarning("(TODO: Remove) Managers search relying on runner search");
                if (NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
                {
                    Debug.LogWarning("Should not be used in a multipeer context, where we could have several peer, and several Managers");
                }
                if (NetworkRunner.Instances.Count > 0)
                {
                    runner = NetworkRunner.Instances[0];
                }
            }
            if (runner != null) managers = runner.GetComponentInChildren<Managers>();
            if (managers == null) managers = FindObjectOfType<Managers>(true);// Should not be used in a multipeer context
            if (managers == null)
            {
                Debug.LogError("Unable to find Managers");
            }
            return managers;
        }
    }
}
