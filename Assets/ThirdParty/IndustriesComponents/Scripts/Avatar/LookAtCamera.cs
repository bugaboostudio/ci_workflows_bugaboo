using Fusion;
using Fusion.XR;
using Fusion.XR.Shared.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 * Rotate the transform to a given target.
 * If constraintAxis is true, only the allowedAxis (global) axis not equal to 0 will be rotated
 * 
 * Performance optimization
 * If visibilityCheckers, and all of them have not visible renderers, the rotation computation won't occur
 * 
 **/

namespace fusion.XR.Tools
{
    public class LookAtCamera : MonoBehaviour
    {
        Camera mainCamera;

        public bool constraintAxis = true;
        public Vector3 allowedAxis = new Vector3(0, 1, 0);

        public RendererVisible[] visibilityCheckers;
        RigInfo rigInfo;

        bool visible = true;

        NetworkObject no;
        private void Awake()
        {
            if (visibilityCheckers.Length > 0)
            {
                visible = false;
                foreach (var checker in visibilityCheckers)
                {
                    checker.onVisibleChange.AddListener(CheckVisibility);
                }
            }
            no = GetComponentInParent<NetworkObject>();
        }

        void CheckVisibility(bool checkerVisibility)
        {
            if (visibilityCheckers.Length > 0)
            {
                visible = false;
                foreach (var checker in visibilityCheckers)
                {
                    if (checker.isVisible)
                    {
                        visible = true;
                        break;
                    }
                }
                if (!visible)
                {
                    return;
                }
            }
        }
        void Update()
        {
            if (!visible) return;

            Transform target = null;
            if (no && no.Runner && rigInfo == null)
            {
                rigInfo = RigInfo.FindRigInfo(no.Runner);
            }
            if (rigInfo && rigInfo.localHardwareRig)
            {
                target = rigInfo.localHardwareRig.headset.transform;
            }
            else
            {
                if (mainCamera == null) mainCamera = Camera.main;
                if (mainCamera) target = mainCamera.transform;
            }
            if (!target) return;

            if (!constraintAxis)
            {
                transform.LookAt(target);
            }
            else
            {
                var direction = target.position - transform.position;
                if (direction.sqrMagnitude > 0.01f)
                {
                    var targetRot = Quaternion.LookRotation(direction);
                    targetRot = Quaternion.Euler(
                        allowedAxis.x == 0 ? 0 : targetRot.eulerAngles.x,
                        allowedAxis.y == 0 ? 0 : targetRot.eulerAngles.y,
                        allowedAxis.z == 0 ? 0 : targetRot.eulerAngles.z);
                    transform.rotation = targetRot;
                }
            }
        }
    }

}
