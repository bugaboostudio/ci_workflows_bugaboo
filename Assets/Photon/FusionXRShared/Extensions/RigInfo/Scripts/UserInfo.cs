using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Fusion.XR.Shared.Rig
{
    /*
     * Synchornize user metadata
     */
    public class UserInfo : NetworkBehaviour
    {

        [Networked(OnChanged = nameof(OnUserNameChange)), Capacity(32)]
        public string UserName { get; set; }

        [Networked(OnChanged = nameof(OnUserAvatarChange)), Capacity(128)]
        public string AvatarURL { get; set; }

        [Header("Events")]
        public UnityEvent onUserNameChange;
        public UnityEvent onUserAvatarChange;

        public static void OnUserNameChange(Changed<UserInfo> changed)
        {
            Debug.Log("[UserInfo] Username changed: " + changed.Behaviour.UserName);
            if (changed.Behaviour.onUserNameChange != null) changed.Behaviour.onUserNameChange.Invoke();
        }

        public static void OnUserAvatarChange(Changed<UserInfo> changed)
        {
            Debug.Log("[UserInfo] Avatar changed: " + changed.Behaviour.AvatarURL);
            if (changed.Behaviour.onUserAvatarChange != null) changed.Behaviour.onUserAvatarChange.Invoke();
        }
    }
}

