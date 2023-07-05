using Fusion.XR.Shared.Locomotion;
using Fusion.XR.Shared.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.XR.Zone
{
    public interface IZoneUserListener
    {
        public void OnZoneChanged(Zone previoius, Zone current);
    }

    public interface INonUserRig { }

    /**
     * If stored in a XRNetworkedRig hierarchy,
     *  Zoneuser will place/remove the rig in Zone when the user moves
     * It can warn IZoneUserListener components of entering/exiting those zones
     */
    public class ZoneUser : MonoBehaviour, ILocomotionValidator, IZoneManagerListener, ILocomotionObserver
    {
        [HideInInspector]
        public NetworkRig rig;
        Vector3 HeadsetPosition => rig.headset != null? rig.headset.transform.position : transform.position;

        public ZoneManager zoneManager;

        public Zone currentZone = null;
        public bool IsInZone => currentZone != null;

        List<IZoneUserListener> listeners = new List<IZoneUserListener>();

        public INonUserRig nonUserRig;

        private void Awake()
        {
            rig = GetComponentInParent<NetworkRig>();
            nonUserRig = rig.GetComponent<INonUserRig>();
        }

        public bool forbidZoneToNonUserRig = true;
        
        private void Start()
        {
            if (zoneManager == null)
                zoneManager = rig.Runner.GetComponentInChildren<ZoneManager>();

            if (zoneManager == null) Debug.LogError("No zone manager. Add one to runner childs");
            zoneManager.RegisterListener(this);

        }

        private void OnDestroy()
        {
            if(zoneManager) zoneManager.UnregisterListener(this);
        }

        public void RegisterListener(IZoneUserListener listener)
        {
            if (listeners.Contains(listener)) return;
            listeners.Add(listener);
        }

        public void UnregisterListener(IZoneUserListener listener)
        {
            if (!listeners.Contains(listener)) return;
            listeners.Remove(listener);
        }

        public void DidEnterZone(Zone zone)
        {
            if (currentZone == zone) return;

            Zone previousZone = currentZone;

            if(previousZone != null)
            {
                previousZone.Exit(rig);
            }

            currentZone = zone;
            foreach (var l in listeners)
            {
                l.OnZoneChanged(previousZone, currentZone);
            }
        }

        public void DidExitZone(Zone zone)
        {
            if (currentZone != zone) return;

            Zone previousZone = currentZone;
            currentZone = null;
            foreach (var l in listeners)
            {
                l.OnZoneChanged(previousZone, currentZone);
            }
        }


        #region ILocomotionObserver
        [ContextMenu("OnDidMove")]
        public void OnDidMove()
        {
            if ((forbidZoneToNonUserRig && nonUserRig != null)) return;

            Zone previousZone = currentZone;
            if(IsInZone)
            {
                if(currentZone.IsRigHeadPositionInZone(HeadsetPosition))
                {
                    // We are still in the same zone
                    return;
                }
                // We left a zone
                currentZone.Exit(rig);
            }
            if(zoneManager)
            {
                foreach (var zone in zoneManager.zones)
                {
                    if (zone.IsRigHeadPositionInZone(HeadsetPosition))
                    {
                        // The head is in a zone, is it possible to enter it ?
                        if (zone.CanEnter(rig))
                        {
                            zone.Enter(rig);
                            break;
                        }
                    }
                }
            }
        }

        public void OnDidMoveFadeFinished()
        {
        }
        #endregion

        #region ILocomotionValidator
        public bool CanMoveHeadset(Vector3 potentialHeadsetPosition)
        {
            if(zoneManager)
            {
                for(int i = 0; i < zoneManager.zones.Count; i++)
                {
                    var zone = zoneManager.zones[i];

                    if (zone.IsRigHeadPositionInZone(potentialHeadsetPosition))
                    {
                        if ( (forbidZoneToNonUserRig && nonUserRig != null) || !zone.CanEnter(rig) )
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        #endregion

        #region IZoneManagerListener
        public void OnRegisterZone(Zone zone)
        {
            // Trigger a OnDidMove() to check if we should enter the new zone
            OnDidMove();
        }

        public void OnUnRegisterZone(Zone zone)
        {
            // Trigger a OnDidMove() to check if we should leave the zone unregistered
            OnDidMove();
        }
        #endregion
    }
}
