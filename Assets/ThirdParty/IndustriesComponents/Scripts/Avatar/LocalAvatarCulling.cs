using Fusion.XR.Shared.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.Samples.IndustriesComponents
{
    /**
     * Ensure that the user camera cannot see its own avatar, by making its layer "invisible"
     */
    [RequireComponent(typeof(HardwareRig))]
    public class LocalAvatarCulling : MonoBehaviour
    {
        public string localAvatarLayer = "InvisibleForLocalPlayer";
        public bool hideLocalAvatar = true;
        HardwareRig rig;

        private void Awake()
        {
            rig = GetComponent<HardwareRig>();
        }

        void ConfigureCamera()
        {
            int layer = LayerMask.NameToLayer(localAvatarLayer);
            if (hideLocalAvatar && layer != -1)
            {
                var camera = rig.headset.GetComponentInChildren<Camera>();
                camera.cullingMask &= ~(1 << layer);
            }
        }

        private void Start()
        {
            // Change camera culling mask to hide local user, if required by hideLocalAvatar
            ConfigureCamera();
        }
    }
}
