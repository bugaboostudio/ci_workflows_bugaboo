using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.XR.Shared.Rig
{
    /**
     * Register in a rig info any component on this gameobject that a rig info stores: HardwareRig (found during Awake) and NetworkRig (found during a spawn)
     * It should not be placed on a NetworkRig gameobject that are not associated with a player (Bots, ...)
     *
     * Note:
     *  This component is a simulation behaviour, to be able to implement ISpawned and receive Spawned calls.
     *  It is not a NetworkBehaviour, as when used to detect a HardwareRig, whe won't have a NetworkObject, so a NetworkBehaviour would be unrelevant
     */
    public class RigInfoRegister : SimulationBehaviour, ISpawned
    {
        public RigInfo rigInfo;

        private void Awake()
        {
            if (TryGetComponent(out HardwareRig hardwareRig))
            {
                if (rigInfo == null) rigInfo = RigInfo.FindRigInfo(hardwareRig.runner);
                rigInfo.localHardwareRig = hardwareRig;
            }
        }

        #region ISpawned
        public void Spawned()
        {
            if(TryGetComponent(out NetworkRig networkRig))
            {
                if (Object.HasInputAuthority)
                {
                    if (rigInfo == null) rigInfo = RigInfo.FindRigInfo(Runner);
                    rigInfo.localNetworkedRig = networkRig;
                }
            }
        }
        #endregion
    }
}

