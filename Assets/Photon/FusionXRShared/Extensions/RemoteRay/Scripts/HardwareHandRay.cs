using Fusion.XR.Shared.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.XR.Shared
{
    /**
     * Store the ray descriptor for this hand, in order to be later shared other the network
     */
    [RequireComponent(typeof(HardwareHand))]
    public class HardwareHandRay : MonoBehaviour
    {
        public IRayDescriptor rayDescriptor;

        private void Awake()
        {
            if(rayDescriptor == null) rayDescriptor = GetComponentInChildren<IRayDescriptor>();
        }
    }
}

