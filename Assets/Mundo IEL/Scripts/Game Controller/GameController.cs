using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Samples.IndustriesComponents;
using Fusion.XR.Shared;
using Fusion.XR.Shared.Rig;
using UnityEngine;

public class GameController : FiniteStateMachine<GameController>
{
    [Header("---- Rig ----")]
    public HardwareRig rig;

    [Header("---- Managers ----")]
    public NetworkRunner mainRunner;
    public ConnectionManager connectionManager;

    [Header("---- Ready Player Me ----")]
    public RPMAvatarLoader rpmLoader;
}
