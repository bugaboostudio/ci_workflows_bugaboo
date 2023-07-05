using Fusion.XR.Shared.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.XR.Shared
{

    [RequireComponent(typeof(HardwareHand))]
    public class HardwareHandRepresentationManager : MonoBehaviour
    {
        public bool displayLocalReprWhenOnline = true;

        HardwareHand hand;
        RigInfo rigInfo;
        public IHandRepresentation localRepresentation;


        private void Awake()
        {
            hand = GetComponent<HardwareHand>();
            var rig = GetComponentInParent<HardwareRig>();
            rigInfo = RigInfo.FindRigInfo(rig.runner);
            // Find any children that can handle hand representaiton (we will forward hand commands to it)
            localRepresentation = GetComponentInChildren<IHandRepresentation>();
        }


        private void Update()
        {
            ManageLocalHandRepresentationDisplay();

            // If a local hand representation is available, we forward the input to it
            //  Note that the hand local representation is also used to store the finger colliders, and so need to be animated even if not having an actively displayed renderer
            if (localRepresentation != null)
            {
                localRepresentation.SetHandCommand(hand.handCommand);
            }
        }

        void ManageLocalHandRepresentationDisplay()
        {
            if (rigInfo != null && rigInfo.localNetworkedRig != null)
            {
                // Online: we hide the local representation of displayLocalReprWhenOnline is false
                if (displayLocalReprWhenOnline != true && localRepresentation != null && localRepresentation.IsMeshDisplayed)
                {
                    localRepresentation.DisplayMesh(false);
                }
            }
            else if (localRepresentation != null && !localRepresentation.IsMeshDisplayed)
            {
                // Offline, but local hand is currently not displayed: we display it
                localRepresentation.DisplayMesh(true);
            }
        }
    }
}
