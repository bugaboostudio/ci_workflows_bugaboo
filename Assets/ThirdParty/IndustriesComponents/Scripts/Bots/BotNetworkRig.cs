using Fusion.XR.Shared.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.Samples.IndustriesComponents
{
    /**
     * Ensure that the local user inputs do not affect this bot moved
     */
    [OrderAfter(typeof(NetworkTransform), typeof(NetworkRigidbody))]
    public class BotNetworkRig : NetworkRig
    {
        public override bool IsLocalNetworkRig => false;

        public override void FixedUpdateNetwork()
        {
        }

        public override void Render()
        {
        }
    }
}

