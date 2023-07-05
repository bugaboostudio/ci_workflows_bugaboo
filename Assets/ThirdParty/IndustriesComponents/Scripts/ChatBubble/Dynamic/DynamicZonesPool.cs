using Fusion;
using Fusion.XR.Zone;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/**
*
* DynamicZonesPool manages the dynamic zones. It maintains a list of used zones and an other list of unused zones.
* When a dynamic zone is not used anymore, it is not destroyed. Instead, the zone is added in the list of unused zone and the display is disable.
* When a dynamic zone is needed, first Pick() should be used to get an avalaible unused zone.
* If none unused zone is available, then, CreateZone() is used to spawn a new dynamic zone.
* DynamicZonesPool Should be stored under the hierarchy of the runner
* 
**/
namespace Fusion.Samples.IndustriesComponents
{
    public class DynamicZonesPool : MonoBehaviour
    {
        // List of dynamic zones, used or not
        public List<DynamicZone> unusedZones = new List<DynamicZone>();
        public List<DynamicZone> usedZones = new List<DynamicZone>();

        // List of rig that can spawn dynamic zones
        public List<DynamicZoneSource> sources;

        public Transform dynamicZonesStorage;

        [Tooltip("At which distance of a DynamicZoneSource an other DynamicZoneSource will trigger a Dynamiczone spawn")]
        public float size = 4;
        [HideInInspector]
        public float sizeSqr;

        public DynamicZone dynamiczonePrefab;

        public Managers managers;

        private void Awake()
        {
            sizeSqr = size * size;
            if (managers == null) managers = Managers.FindInstance();
            if (dynamicZonesStorage == null)
            {
                var storage = new GameObject("DynamicZonesStorage");
                dynamicZonesStorage = storage.transform;
            }
        }

        // CreateZone is in charge to spawn a dynamic Zone
        // The PhotonVoiceInterestGroup is set randomly
        public async Task<DynamicZone> CreateZone()
        {
            // initialized a list of potential group number
            var possibleGroups = new List<int>(System.Linq.Enumerable.Range(11, 254));

            // remove a group number from the list if a unsued zone has the same number for the PhotonVoiceInterestGroup parameter
            foreach (var zone in unusedZones) possibleGroups.Remove(zone.zone.PhotonVoiceInterestGroup);

            // Check if it is still possible to create a zone (PhotonVoiceInterestGroup available)
            if (possibleGroups.Count > 0)
            {
                // create a random PhotonVoiceInterestGroup number
                int interestGroup = possibleGroups[Random.Range(0, possibleGroups.Count - 1)];
                bool creatingZone = true;

                // Instantiate the dynamic zone prefab
                var dynamicZone = managers.runner.Spawn(dynamiczonePrefab, onBeforeSpawned: (runner, obj) =>
                {
                    creatingZone = false;
                });
                while (creatingZone)
                {
                    await Task.Delay(10);
                }
                // configure the PhotonVoiceInterestGroup
                dynamicZone.zone.PhotonVoiceInterestGroup = (byte)(interestGroup);
                // return the dynmaic zone
                return dynamicZone;
            }
            // return null because it was not possible to find a PhotonVoiceInterestGroup available
            return null;
        }

        // If a zone is avalaible (not used), Pick activates the zone & return it
        public DynamicZone Pick()
        {
            // Check if a zone is available
            if (unusedZones.Count == 0)
            {
                return null;
            }
            // Initialized and return an available dynamic zone
            DynamicZone freeZone = unusedZones[unusedZones.Count - 1];
            Activate(freeZone);
            return freeZone;
        }

        // When a dynamic zone is spwaned, it register itself into the unused zone list
        public void Register(DynamicZone zone)
        {
            // Exit if the zone is already registered in used or unused zones
            if (usedZones.Contains(zone) || unusedZones.Contains(zone)) return;

            // Add the zone into the unused zones list
            unusedZones.Add(zone);
        }

        // Activate updates the used/unused zone lists and registers the zone into the zone manager
        public void Activate(DynamicZone zone)
        {
            // Check if the zone was already used
            if (usedZones.Contains(zone))
            {
                // the zone was already used
                // do nothing
                return;
            }
            // The zone was not used
            // Remove the zone from the unused zone list
            unusedZones.Remove(zone);
            // Add the zone into the used zone list
            usedZones.Add(zone);
            // Register the zone into the zone manager
            zone.zone.RegisterInZoneManager();
        }


        // Deactivate updates the used/unused zone lists and unregisters the zone from the zone manager
        public void Deactivate(DynamicZone zone)
        {
            // Check if the zone is into the used zone list
            if (!usedZones.Contains(zone))
            {
                // the zone was not used
                // do nothing
                return;
            }
            // The zone was used
            // Unregister the zone from the zone manager
            zone.zone.UnregisterInZoneManager();
            // Remove the zone from the used zone list
            usedZones.Remove(zone);
            // Add the zone into the unused zone list
            unusedZones.Add(zone);
        }
    }
}
