using Fusion;
using Fusion.XR;
using Fusion.XR.Zone;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/**
 * 
 * Each player (NetworkedRig) is a dynamic zone source (bot are not). So, when the player is spawned, it register itself in the pool of zone source.
 * If two players are in proximity, the player with the lower network id creates a dynamic zone arround them (if not yet created). 
 * This dynamic zone can be pick up from the list of unused zone or can be created by the zone manager if none is available.
 *  
 **/

namespace Fusion.Samples.IndustriesComponents
{

    [RequireComponent(typeof(ZoneUser))]
    public class DynamicZoneSource : SimulationBehaviour, ISpawned, IZoneUserListener
    {
        [HideInInspector]
        public ZoneUser zoneUser;
        bool registeredInPool = false;
        public Managers managers;

        private void Awake()
        {
            zoneUser = GetComponent<ZoneUser>();
            zoneUser.RegisterListener(this);
        }

        // Register the player in the list of zone sources
        public void RegisterInPool()
        {
            // Exit if the player is already registered
            if (registeredInPool) return;

            // Add the player in the list of zone sources
            managers.dynamicZonespool.sources.Add(this);
            registeredInPool = true;
        }

        // Remove the player in the list of zone sources
        public void UnregisterFromPool()
        {
            // Check if the player was been unregisterd
            if (!registeredInPool) return;

            // Remove the player in the list
            managers.dynamicZonespool.sources.Remove(this);
            registeredInPool = false;
        }

        // Unregister the player from the pool when he is destroyed
        private void OnDestroy()
        {
            UnregisterFromPool();
        }

        // Register the player in the zone pool when he is spawned
        public void Spawned()
        {
            if (managers == null) managers = Managers.FindInstance(Object.Runner);
            RegisterInPool();
        }

        // We do not register bot as zone sources
        public bool IsValidSource => zoneUser.nonUserRig == null;

        // Check if a dynamic zone should be created
        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            // Check if the network rig is a player or a bot
            if (!IsValidSource)
            {
                // It is a bot, unregister it from the pool
                UnregisterFromPool();
                return;
            }

            // Check if a dynamic zone should be created
            CheckAllocatedZone();
        }

        float proximityStartTime = -1;
        DynamicZoneSource proximitySource;
        float maxProximityDuration = 0.2f;

        public bool allocatingZone = false;

        // CheckAllocatedZone is in charge to verify if the local player is in proximity with another source (player)
        // If two player are in proximity, the player with the lower network id creates a dynamic zone arround them (if not yet created)
        async void CheckAllocatedZone()
        {
            // We only handle dynamic zone creation for the local user (non bot)
            if (!zoneUser.rig.Object.HasStateAuthority || zoneUser.nonUserRig != null) return;

            // Check if the user is already in a zone or if an allocation is in progress
            if (!allocatingZone && zoneUser.currentZone == null)
            {
                // the user can be added in a zone
                // reset the proximity flag
                bool proximityDetected = false;

                // Check if we should allocate a zone
                for (int i = 0; i < managers.dynamicZonespool.sources.Count; i++)
                {
                    var sourceRig = managers.dynamicZonespool.sources[i];
                    // Skip if the source is not valid
                    if (sourceRig == this || sourceRig.IsValidSource == false)
                    {
                        continue;
                    }
                    if (sourceRig.zoneUser.currentZone != null)
                    {
                        if (proximitySource == sourceRig)
                        {
                            proximityStartTime = -1;
                        }
                        continue;
                    }

                    // Here, source is valid
                    // Check if the user is near of this source
                    if ((sourceRig.zoneUser.rig.headset.transform.position - zoneUser.rig.headset.transform.position).sqrMagnitude < managers.dynamicZonespool.sizeSqr)
                    {
                        // Both sources are in proximity
                        bool proximityForThisSourceStillActive = true;

                        // Check if the proximity duration timer has been initialized
                        if (proximityStartTime == -1)
                        {
                            // proximity just start

                            // We determine around which source the zone will be spawned: the one with the lowest id if we both are still or at close speed
                            bool shouldInitZone = false;
                            if (zoneUser.rig.Object.StateAuthority.PlayerId < sourceRig.zoneUser.rig.Object.StateAuthority.PlayerId)
                            {
                                shouldInitZone = true;
                            }
                            else
                            {
                                shouldInitZone = false;
                            }

                            if (shouldInitZone)
                            {
                                // We are the slower one 
                                // Initialize the timer
                                proximityStartTime = Time.time;
                                proximitySource = sourceRig;
                            }
                        }
                        // proximity timer is already initialized, check the duration
                        else if ((Time.time - proximityStartTime) > maxProximityDuration)
                        {
                            // the proximity duration exceed the threshold
                            // a dynamic zone must be created
                            allocatingZone = true;
                            proximityForThisSourceStillActive = false;
                            // ask for zone
                            Zone allocatedZone = await Pick();

                            if (sourceRig.zoneUser.currentZone != null && sourceRig.zoneUser.currentZone != allocatedZone)
                            {
                                // The user entered another zone, cancelling
                                allocatingZone = false;
                                break;
                            }

                            if (zoneUser.currentZone != null && zoneUser.currentZone != allocatedZone)
                            {
                                // The user entered another zone, cancelling
                                allocatingZone = false;
                                break;
                            }

                            // We place the picked allocated zone between the 2 users
                            allocatedZone.transform.position = (transform.position + sourceRig.transform.position) / 2;

                            // Add both source in the dynamic zone
                            allocatedZone.Enter(zoneUser.rig);
                            allocatedZone.Enter(sourceRig.zoneUser.rig);
                            allocatingZone = false;
                        }
                        // update the proximity flag
                        proximityDetected = proximityDetected || proximityForThisSourceStillActive;
                    }

                    // check if a proximity between two source has been found
                    if (proximityDetected)
                    {   // proximity has been found
                        // exit as we don't want to spawn multiple zones if several users are in proximity
                        break;
                    }
                }

                // no proximity found during this verification
                if (!proximityDetected)
                {
                    // reset the timer
                    proximityStartTime = -1;
                }
            }
        }

        //  Pick is in charge to return a zone. If there is no dynamic zone available, then it ask to create a new zone 
        async Task<Zone> Pick()
        {
            // Ask the zone pool manager for an unused dynamic zone
            DynamicZone dynamicZone = managers.dynamicZonespool.Pick();

            // Check if a dynamic zone has been affected
            if (!dynamicZone)
            {
                // no unused dynamic zone available
                // ask to create a new zone
                dynamicZone = await managers.dynamicZonespool.CreateZone();
            }

            // Check if a dynamic zone has been affected
            if (dynamicZone)
            {
                // a dynamic zone has been affected
                Zone zone = dynamicZone.zone;
                // ask for the authority
                await zone.Object.RequestAllAuthority();
                dynamicZone.IsUsed = true;
                // Update position, rotation & visibility
                zone.transform.position = transform.position;
                zone.transform.rotation = transform.rotation;
                zone.IsVisible = true;
                // return the zone
                return zone;
            }
            // Return null because it was not possible to get a dynamic zone
            else
            {
                Debug.LogError("No free zone to pick");
            }
            return null;
        }

        #region IZoneUserListener
        public void OnZoneChanged(Zone previoius, Zone current)
        {
            if (current != null)
            {
                // We are entering a zone: if we were planning to create a zone due to a proximity, we cancel it
                proximityStartTime = -1;
                proximitySource = null;
            }
        }
        #endregion
    }
}
