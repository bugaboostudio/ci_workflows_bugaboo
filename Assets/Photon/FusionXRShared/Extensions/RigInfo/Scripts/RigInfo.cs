using Fusion.XR.Shared.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.XR.Shared.Rig
{
    [System.Serializable]
    public struct UserStartInfo
    {
        public string name;
        public string avatarURL;
    }

    public class RigInfo : MonoBehaviour
    {
        public const string SETTINGS_AVATARURL = "avatarURL";
        public const string SETTINGS_USERNAME = "userName";


        [Header("Local rigs")]
        public HardwareRig localHardwareRig;
        public NetworkRig localNetworkedRig;
        
        [Header("Local user info")]
        public UserStartInfo localUserStartInfo;
        public bool loadSavedAvatarSettings = true;

        private void Awake()
        {
            if (loadSavedAvatarSettings)
            {
                // Load saved user info
                string avatarURL = PlayerPrefs.GetString(SETTINGS_AVATARURL);
                if (avatarURL != null && avatarURL != "")
                {
                    localUserStartInfo.avatarURL = avatarURL;
                }
                string userName = PlayerPrefs.GetString(SETTINGS_USERNAME);
                if (userName != null && userName != "")
                {
                    localUserStartInfo.name = userName;
                }
            }
        }

        /**
         * Look for a RigInfo, under the runner hierarchy
         */
        public static RigInfo FindRigInfo(NetworkRunner runner = null)
        {
            RigInfo rigInfo = null;
            if (runner != null) rigInfo = runner.GetComponentInChildren<RigInfo>();
            if (rigInfo == null)
            {
                Debug.LogWarning("Unable to find RigInfo: it should be stored under the runner hierarchy");
            }
            return rigInfo;
        }
    }
}

