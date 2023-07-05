using Fusion.XR.Shared.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.XR.Shared.Locomotion
{
    /*
     * Handles view fading related to movement validation
     * If checkHeadMovements and fadeIfMovingHeadInInvalidZone are true, when the user moves its head in an invalid zone (according to any ILocomotionValidator)
     *  the view will fade until they go out of the invalid zone.
     */
    [RequireComponent(typeof(HardwareRig))]
    public class InvalidMoveCameraFader : MonoBehaviour, ILocomotionObserver
    {
        [Header("Headset movement analysis (for zones handling mainly)")]
        public bool fadeIfMovingHeadInInvalidZone = true;

        bool isInInvalidZone = false;
        HardwareRig rig;
        HardwareLocomotionValidation hardwareLocomotionValidation;

        private void Awake()
        {
            rig = GetComponent<HardwareRig>();
            hardwareLocomotionValidation = GetComponent<HardwareLocomotionValidation>();
        }

        #region ILocomotionObserver
        // Check if the head enters a forbidden Zone, and fade the view if it occurs (if checkHeadMovements is set to ture)
        public void OnDidMove()
        {
            if (fadeIfMovingHeadInInvalidZone && rig.headset.fader)
            {
                if (!hardwareLocomotionValidation.CanMoveHeadset(rig.headset.transform.position))
                {
                    if (!isInInvalidZone)
                    {
                        isInInvalidZone = true;
                        StartCoroutine(rig.headset.fader.FadeIn());
                    }
                }
                else
                {
                    if (isInInvalidZone)
                    {
                        isInInvalidZone = false;
                        StartCoroutine(rig.headset.fader.FadeOut());
                    }
                }
            }
        }

        public void OnDidMoveFadeFinished()
        {
        }
        #endregion
    }
}


