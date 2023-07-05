using System.Collections;
using System.Collections.Generic;
using Fusion.XR.Shared.Rig;
using UnityEngine;

public class IEL_NetworkRig : NetworkRig
{
    public override void FixedUpdateNetwork()
    {
        // base.FixedUpdateNetwork();

        // update the rig at each network tick
        
    }

    protected override void ApplyInputToRigParts(RigInput input)
    {
        //transform.position = input.playAreaPosition;
        //transform.rotation = input.playAreaRotation;
    }

    protected override void ApplyInputToHandPoses(RigInput input)
    {
    }

    public override void Render()
    {
        if (IsLocalNetworkRig)
        {
            // Extrapolate for local user :
            // we want to have the visual at the good position as soon as possible, so we force the visuals to follow the most fresh hardware positions
            // To update the visual object, and not the actual networked position, we move the interpolation targets
            // networkTransform.InterpolationTarget.position = hardwareRig.transform.position;
            // networkTransform.InterpolationTarget.rotation = hardwareRig.transform.rotation;
            networkTransform.transform.position = hardwareRig.transform.localPosition;
            networkTransform.transform.rotation = hardwareRig.transform.localRotation;
        }
    }
}
