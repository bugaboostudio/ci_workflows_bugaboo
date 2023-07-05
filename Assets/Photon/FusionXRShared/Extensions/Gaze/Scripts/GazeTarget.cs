using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 * Represents an object that can be followed by Gazers, to emulate eye tracking
 * 
 **/

namespace Fusion.XR
{

    public class GazeTarget : MonoBehaviour
    {
        public enum Status { 
            Normal,
            Active,// moving, speaking, ...
            Event, // need as specific attention - started speaking,...
            Inactive // lack of current interest - no move, no voice, ...
        }
        public Status status = Status.Normal;

        public GazeInfo gazeInfo;

        private void Start()
        {
            if (gazeInfo == null)
            {
                NetworkObject no = GetComponentInParent<NetworkObject>();
                if (no && no.Runner) gazeInfo = no.Runner.GetComponentInChildren<GazeInfo>();
            }
            if (gazeInfo == null) gazeInfo = FindObjectOfType<GazeInfo>();
            if(gazeInfo) gazeInfo.RegisterGazeTarget(this);
        }

        private void OnDestroy()
        {
            if(gazeInfo) gazeInfo.UnregisterGazeTarget(this);
        }
    } 
}
