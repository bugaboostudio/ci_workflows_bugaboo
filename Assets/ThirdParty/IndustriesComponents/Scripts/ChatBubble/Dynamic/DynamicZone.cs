using Fusion;
using Fusion.XR.Zone;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/**
 * 
 * When the DynamicZone is created, it register itself into the zone manager and it is added into the unused zone.
 * When two players are in proximity, the player with the slower velocity spawn a dynamic zone arround him (DynamicZoneSource).
 * DynamicZone checks if the dynamic zone is still in used (at least two players into the zone)
 * If the dynamic zone must be released, then the zone manager is notified to update the used/unused zone lists
 * 
 **/

namespace Fusion.Samples.IndustriesComponents
{
    [RequireComponent(typeof(Zone))]
    public class DynamicZone : NetworkBehaviour
    {

        [Networked(OnChanged = nameof(OnIsUsedChanged))]
        public NetworkBool IsUsed { get; set; }
        public Managers managers;

        public Zone zone;
        float allocatedUnusedStartTime = -1;
        float maxAllocatedUnusedDuration = 1f;
        public bool displayDomeIfNotLocked = false;

        private void Awake()
        {
            zone = GetComponent<Zone>();
            zone.registerAutomatically = false;
        }

        public override void Spawned()
        {
            base.Spawned();
            zone.InstantChangeVisibility(false);
            if (managers == null) managers = Managers.FindInstance(Object.Runner);

            // Add the zone into the unused zones list
            managers.dynamicZonespool.Register(this);

            // move the dynamic zone in the hierarchy
            if (managers.dynamicZonespool.dynamicZonesStorage) transform.parent = managers.dynamicZonespool.dynamicZonesStorage;
            name += "-" + zone.PhotonVoiceInterestGroup;

        }

        // OnIsUsedChanged is call when the dynamic zone bool IsUsed is changed
        static void OnIsUsedChanged(Changed<DynamicZone> changed)
        {
            changed.Behaviour.NotifyPool();
        }

        // NotifyPool updates the used/unused lists
        void NotifyPool()
        {
            if (IsUsed)
            {
                // Remove the zone from the unused zone list and add it into the used list
                managers.dynamicZonespool.Activate(this);
            }
            else
            {
                // Remove the dynamic zone from the used list and add it into the unused list
                managers.dynamicZonespool.Deactivate(this);
            }
        }

        // Check if the dynamic zone is still in used (at least two players)
        // if the dynamic zone must be released, then the zone manager is notified
        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            // Do nothing if we do not have the state authority
            if (!Object.HasStateAuthority) return;

            // Check if we should release the allocated zone
            if (IsUsed && zone.RigsInzone.Count <= 1)
            {
                // only one player in the zone
                // check if the timer has started
                if (allocatedUnusedStartTime == -1)
                {
                    // Initialize the timer
                    allocatedUnusedStartTime = Time.time;
                }
                else
                {
                    // unused timer is already initialized, check the duration
                    if ((Time.time - allocatedUnusedStartTime) > maxAllocatedUnusedDuration)
                    {
                        // the unused duration exceed the threshold
                        // the dynamic zone must be released
                        Release();
                    }
                }
            }
            else
            {
                // the dynamic zone is still in used
                // reset the timer
                allocatedUnusedStartTime = -1;
            }
        }

        // Release update the dynamic used status and disable the display
        void Release()
        {
            // Inform the pool manager that the zone is not used anymore (OnIsUsedChanged => NotifyPool() )
            IsUsed = false;
            // stop displaying the dynamic zone
            zone.IsVisible = false;
        }
    }
}
