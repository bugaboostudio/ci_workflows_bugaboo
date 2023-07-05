using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.XR.Shared.Rig
{
    /*
    * Load local user info into UserInfo
    * It should not be placed on a NetworkRig gameobject that are not associated with a player (Bots, ...)
    */
    public class LocalUserInfoLoader : NetworkBehaviour
    {
        public override void Spawned()
        {
            base.Spawned();
            if (Object.HasInputAuthority)
            {
                RigInfo rigInfo = RigInfo.FindRigInfo(Runner);
                if (TryGetComponent<UserInfo>(out UserInfo userInfo))
                {
                    // For the localuser, we set up the user name and avatar url with the potential data stored in the rig info localUserStartInfo
                    userInfo.UserName = rigInfo.localUserStartInfo.name;
                    userInfo.AvatarURL = rigInfo.localUserStartInfo.avatarURL;
                }
            }
        }
    }

}
