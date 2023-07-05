using Fusion.XR.Shared.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.XR.Shared.Locomotion
{
    /**
     * Used by HardwareLocomotionValidation if some network component (on the local user prefab) are validator or observers, to forward the call to them
     * Detect if the rig has actually moved to trigger OnDidMove
     */
    [RequireComponent(typeof(NetworkRig))]
    [OrderAfter(typeof(NetworkTransform), typeof(NetworkRigidbody), typeof(NetworkRig))]
    public class NetworkLocomotionValidation : ChildrenBasedLocomotionValidation
    {
        float minimalDetectedMoveSqr = 0.025f;
        Vector3 lastCheckedPosition;

        NetworkRig rig;

        protected override void Awake()
        {
            base.Awake();
            rig = GetComponent<NetworkRig>();
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            bool didMoveDuringFUN = false;

            // Detect minimal movements, to trigger OnDidMove
            var move = rig.transform.position - lastCheckedPosition;
            if (move.sqrMagnitude >= minimalDetectedMoveSqr)
            {
                didMoveDuringFUN = true;
            }

            lastCheckedPosition = rig.transform.position;
            if (didMoveDuringFUN)
            {
                OnDidMove();
            }
        }
    }
}
