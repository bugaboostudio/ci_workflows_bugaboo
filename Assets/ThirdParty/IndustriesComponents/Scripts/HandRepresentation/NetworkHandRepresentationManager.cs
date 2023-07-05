using Fusion.XR.Shared.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.XR.Shared
{
    public class NetworkHandRepresentationManager : MonoBehaviour
    {
        public bool displayForLocalPlayer = false;
        NetworkRig networkRig;
        public IHandRepresentation handRepresentation;

        private void Awake()
        {
            networkRig = GetComponent<NetworkRig>();
            if (handRepresentation == null) handRepresentation = GetComponentInChildren<IHandRepresentation>();
        }

        private void Update()
        {
            ManageLocalHandRepresentationDisplay();
        }

        void ManageLocalHandRepresentationDisplay()
        {
            if (networkRig && networkRig.IsLocalNetworkRig)
            {
                // This hand is associated to the local user. We manage its display accordingly to displayForLocalPlayer
                if (displayForLocalPlayer != true && handRepresentation != null && handRepresentation.IsMeshDisplayed)
                {
                    handRepresentation.DisplayMesh(false);
                }
                else if (displayForLocalPlayer == true && handRepresentation != null && !handRepresentation.IsMeshDisplayed)
                {
                    handRepresentation.DisplayMesh(true);
                }
            }
        }
    }
}

