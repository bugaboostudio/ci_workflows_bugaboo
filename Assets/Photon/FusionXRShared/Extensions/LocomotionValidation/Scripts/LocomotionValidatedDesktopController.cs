using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Shared.Locomotion;
using UnityEngine.AI;

namespace Fusion.XR.Shared.Desktop
{
    public class LocomotionValidatedDesktopController : DesktopController
    {
        HardwareLocomotionValidation hardwareLocomotionValidation;
        NavMeshAgent agent;

        private void Awake()
        {
            hardwareLocomotionValidation = GetComponent<HardwareLocomotionValidation>();
            agent = GetComponent<NavMeshAgent>();
            locomotion = GetComponentInChildren<RigLocomotion>();
            if (agent)
            {
                agent.updatePosition = false;
                agent.updateRotation = false;
            }
        }

        public override void Move(Vector3 newPosition)
        {
            var move = newPosition - rig.transform.position;
            var newHeadsetPosition = rig.headset.transform.position + move;
            // Check if the validators accept this new rig position
            if (!hardwareLocomotionValidation.CanMoveHeadset(newHeadsetPosition)) return;

            // check if DesktopController validation is ok for this new position too
            if (IsValidHeadPosition(newHeadsetPosition) || !IsValidHeadPosition(rig.headset.transform.position))
            {
                base.Move(newPosition);
            }
        }

        /**
         * Determine if a head position is valid, by checking:
         * - if it would be in a non trigger collider
         * - if it will be above a navmesh point
         */
        public bool IsValidHeadPosition(Vector3 targetPos)
        {
            Collider[] headColliders = Physics.OverlapBox(targetPos, 0.2f * Vector3.one, Quaternion.identity);

            foreach (var c in headColliders)
            {
                if (c.isTrigger == false)
                {
                    return false;
                }
            }
            if (agent)
            {
                var ray = new Ray(targetPos, -transform.up);
                if (Physics.Raycast(ray, out var hit, 100f, locomotion.locomotionLayerMask))
                {
                    if (NavMesh.SamplePosition(hit.point + transform.up * 0.1f, out var navMeshHit, 0.8f, NavMesh.AllAreas))
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            return true;
        }
    }

}
