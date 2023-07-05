using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.XR.Zone
{
    public interface IZoneManagerListener
    {
        public void OnRegisterZone(Zone zone);
        public void OnUnRegisterZone(Zone zone);
    }

    /**
     * Store the current zone to observe for ZoneUser omponents
     * Must be placed under the NetworkRunner hierarchy
     */
    public class ZoneManager : MonoBehaviour
    {
        public List<Zone> zones;
        public List<IZoneManagerListener> listeners = new List<IZoneManagerListener>();

        public void RegisterZone(Zone zone)
        {
            if (zones.Contains(zone)) return;
            zones.Add(zone);
            foreach (var listener in listeners) listener.OnRegisterZone(zone);
        }
        public void UnregisterZone(Zone zone)
        {
            if (!zones.Contains(zone)) return;
            zones.Remove(zone);
            foreach (var listener in listeners) listener.OnUnRegisterZone(zone);
        }

        public void RegisterListener(IZoneManagerListener listener)
        {
            if (listeners.Contains(listener)) return;
            listeners.Add(listener);
        }

        public void UnregisterListener(IZoneManagerListener listener)
        {
            if (!listeners.Contains(listener)) return;
            listeners.Remove(listener);
        }
    }
}
