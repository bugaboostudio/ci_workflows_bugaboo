using Fusion.XR.Shared;
using Fusion.XR.Shared.Desktop;
using Fusion.XR.Shared.Rig;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.XR;

/**
 * 
 * RandomizeStartPosition is in charge to spawn users at a random position 
 * 
 **/

namespace Fusion.Samples.IndustriesComponents
{
    public class RandomizeStartPosition : MonoBehaviour
    { 
        public Transform startCenterPosition;
        public float randomRadius = 10;

        [Header("VR")]
        public bool shouldAlignHeadsetInsteadOfRigInVR = true;

        public Transform childHeadsetToAlign = null;
        public InputActionProperty headsetAvailableAction = new InputActionProperty();

        bool positionSet = false;
        private void Awake()
        {
            if (GetComponent<DesktopController>())
            {
                // Not in VR
                shouldAlignHeadsetInsteadOfRigInVR = false;
            } else
            {
                // VR
                var headset = GetComponentInChildren<HardwareHeadset>();
                if(headset && childHeadsetToAlign == null) childHeadsetToAlign = headset.transform;
            }

            FindStartPosition();
        }

        public void FindStartPosition() { 

            int tries = 0;
            if (startCenterPosition == null) startCenterPosition = transform;

            // try to 10 times to find a valid destination point near the random position
            while (tries < 10)
            {
                Vector3 pos = startCenterPosition.position + randomRadius * Random.insideUnitSphere;
                pos = new Vector3(pos.x, startCenterPosition.position.y, pos.z);
                // check if a destination has been found near the random position
                if (NavMesh.SamplePosition(pos, out var hit, 1f, NavMesh.AllAreas))
                {
                    transform.position = hit.position;
                    transform.rotation = startCenterPosition.rotation;
                    Debug.Log("Placed rig at start position : " + transform.position );
                    break;
                }
                tries++;
            }
            positionSet = true;

            headsetAvailableAction.EnableWithDefaultXRBindings(new List<string> { "<XRHMD>/trackingState" } );
        }

        void Start()
        {
            AlignHeadsetInVR();
        }


        InputTrackingState TrackingState => (InputTrackingState)headsetAvailableAction.action.ReadValue<int>();
        
        async void AlignHeadsetInVR()
        {
            if (!shouldAlignHeadsetInsteadOfRigInVR) return;
            if (!childHeadsetToAlign) return;

            while (!positionSet) await Task.Delay(10);

            float waitEnd = Time.time + 10;
            while (TrackingState == InputTrackingState.None && Time.time < waitEnd) await Task.Delay(100);
            if (TrackingState == InputTrackingState.None)
            {
                Debug.LogError("Wait timeout for VR headset detection");
                return;
            }

            var localChildrotation = Quaternion.Inverse(transform.rotation) * childHeadsetToAlign.rotation;
            var alignRotation = startCenterPosition.rotation * Quaternion.Inverse(localChildrotation);
            transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, alignRotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            Debug.Log("Aligned headset with start position");
        }
    }
}
