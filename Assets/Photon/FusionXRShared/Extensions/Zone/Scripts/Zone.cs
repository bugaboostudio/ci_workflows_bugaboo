using Fusion.Sockets;
using Fusion.XR.Shared.Rig;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.XR.Zone
{
    public interface IZoneListener
    {
        public void OnZoneChanged(Zone zone);
        public void OnLockChange(Zone zone);
        public void OnZoneDestroyed(Zone zone);

        public void OnZoneUserEnterZone(ZoneUser zoneUser);
        public void OnZoneUserExitZone(ZoneUser zoneUSer);

        public void InstantChangeZoneVisibility(Zone zone, bool visible);
        public IEnumerator ChangeZoneVisibility(Zone zone, bool visible);
        public void DidEndChangeZoneVisibility(Zone zone, bool visible);
    }

    /**
     * Defines a zone where a rig could enter.
     * This zone could have a max capacity (or could be manually locked) preventing further users to go there.
     * The locomotion system can use it to validate destination, as well as audio management for interest group
     */
    public class Zone : NetworkBehaviour, INetworkRunnerCallbacks
    {
        [Networked(OnChanged = nameof(OnRigsInZoneChanged)), Capacity(20)]
        public NetworkLinkedList<NetworkBehaviourId> RigsInzone { get; }

        [Networked(OnChanged = nameof(OnForceLockedChanged))]
        public NetworkBool IsForceLocked { get; set; }

        [Networked(OnChanged = nameof(OnIsVisibleChanged))]
        public NetworkBool IsVisible { get; set; }

        public ZoneManager zoneManager;
        public bool registerAutomatically = true;
        public bool isRegistered = false;

        public bool CanAcceptNewRig => RigsInzone.Count < maxCapacity && !IsForceLocked;

        List<IZoneListener> zoneListeners = new List<IZoneListener>();

        [Header("Audio interest group")]
        public bool hasAudioInterestGroup = false;

        [Networked]
        public byte PhotonVoiceInterestGroup { get; set; }

        public virtual bool IsGlobalAudioGroup => hasAudioInterestGroup && PhotonVoiceInterestGroup == 0;

        bool spawned = false;

        public enum ZoneShape
        {
            SphereZone, // a user is in if its head is in a sphere
            CircleZone, // a user is in if its head prjection on the ground is in a circle
            ColliderBounds  // a user is in if its head is in the collider bounds
        }
        [Header("Zone description")]
        public ZoneShape shape = ZoneShape.SphereZone;
        [DrawIf(nameof(shape), (int)ZoneShape.ColliderBounds, Hide=true)]
        public Collider boundCollider;

        [Tooltip("At which distance of the Zone center a ZoneUser triggers an enter/exit")]
        public float size = 3;
        float sizeSqr;
        public int maxCapacity = 10;
        public bool allowEmptyZoneLocking = false;
        // Center of the zone. The up direction is important to determine gorund projection for the CircleZone shape
        public Transform centerTransform;

        private void Awake()
        {
            if (centerTransform == null) centerTransform = transform;
            sizeSqr = size * size;
        }

        public override void Spawned()
        {
            base.Spawned();
            spawned = true;
            if (zoneManager == null)
                zoneManager = Runner.GetComponentInChildren<ZoneManager>();

            if (zoneManager == null) Debug.LogError("No zone manager. Add one to runner childs");

            if (registerAutomatically)
            {
                RegisterInZoneManager();
            }
            Runner.AddCallbacks(this);

            OnZoneChange();
        }

        public void RegisterInZoneManager()
        {
            if (zoneManager)
            {
                zoneManager.RegisterZone(this);
                isRegistered = true;
            }
        }
        public void UnregisterInZoneManager()
        {
            if (zoneManager)
            {
                zoneManager.UnregisterZone(this);
                isRegistered = false;
            }
            if (!spawned) return;
            foreach (var rig in RigsInzone)
            {
                // We expulse user when the zone is unregistered
                ChangeRigPresence(false, rig);
            }
        }

        public ZoneUser ZoneUserForRigId(NetworkBehaviourId rigId)
        {
            if (Runner.TryFindBehaviour(rigId, out NetworkBehaviour rig))
            {
                ZoneUser zoneUser = rig.GetComponentInChildren<ZoneUser>();
                if (zoneUser) return zoneUser;
            }
            return null;
        }

        void UpdateZoneUsers(List<NetworkBehaviourId> exitRigs)
        {
            foreach (var rigId in exitRigs)
            {
                ZoneUser zoneUser = ZoneUserForRigId(rigId);
                if (zoneUser)
                {
                    zoneUser.DidExitZone(this);
                    foreach (var listener in zoneListeners) listener.OnZoneUserExitZone(zoneUser);
                }
            }
            foreach (var rigId in RigsInzone)
            {
                ZoneUser zoneUser = ZoneUserForRigId(rigId);
                if (zoneUser)
                {
                    zoneUser.DidEnterZone(this);
                    foreach (var listener in zoneListeners) listener.OnZoneUserEnterZone(zoneUser);
                }
            }
        }

        static void OnRigsInZoneChanged(Changed<Zone> changed)
        {
            var currentRigs = changed.Behaviour.RigsInzone;
            changed.LoadOld();
            var previousRigs = changed.Behaviour.RigsInzone;
            changed.LoadNew();
            List<NetworkBehaviourId> exitRigs = new List<NetworkBehaviourId>();
            foreach (var previousRig in previousRigs)
            {
                if(!currentRigs.Contains(previousRig))
                {
                    exitRigs.Add(previousRig);
                }
            }
            changed.Behaviour.UpdateZoneUsers(exitRigs);
            changed.Behaviour.CheckStatus();
        }

        static void OnForceLockedChanged(Changed<Zone> changed)
        {
            changed.Behaviour.CheckStatus();
            changed.Behaviour.OnLockChange();
        }

        static void OnIsVisibleChanged(Changed<Zone> changed)
        {
            changed.Behaviour.ChangeVisibility(changed.Behaviour.IsVisible);
        }

        public void RegisterListener(IZoneListener listener)
        {
            if (zoneListeners.Contains(listener)) return;
            zoneListeners.Add(listener);
        }

        public void UnregisterListener(IZoneListener listener)
        {
            if (!zoneListeners.Contains(listener)) return;
            zoneListeners.Remove(listener);
        }

        public bool IsRigHeadPositionInZone(Vector3 headPosition)
        {
            if (shape == ZoneShape.SphereZone)
            {
                return (centerTransform.position - headPosition).sqrMagnitude <= sizeSqr;
            }
            if (shape == ZoneShape.CircleZone)
            {
                var groundPosition = centerTransform.InverseTransformPoint(headPosition);
                groundPosition = new Vector3(groundPosition.x, 0, groundPosition.z);
                return groundPosition.sqrMagnitude <= sizeSqr;
            }
            if (shape == ZoneShape.ColliderBounds)
            {
                return boundCollider.bounds.Contains(headPosition);
            }
            return false;
        }

        public void InstantChangeVisibility(bool visible)
        {
            foreach (var l in zoneListeners)
            {
                if (l != null) l.InstantChangeZoneVisibility(this, visible);
            }
            foreach (var l in zoneListeners)
            {
                if (l != null) l.DidEndChangeZoneVisibility(this, visible);
            }
        }

        void ChangeVisibility(bool visible)
        {
            StartCoroutine(ChangeVisibilityCoroutine(visible));
        }

        public IEnumerator ChangeVisibilityCoroutine(bool visible)
        {
            foreach (var l in zoneListeners)
            {
                if (l != null) yield return l.ChangeZoneVisibility(this, visible);
            }
            foreach (var l in zoneListeners)
            {
                if (l != null) l.DidEndChangeZoneVisibility(this, visible);
            }
        }


        public bool CanEnter(NetworkRig rig)
        {
            if (RigsInzone.Contains(rig.Id) == true) return true;
            if (!CanAcceptNewRig) return false;
            return true;
        }

        public void Enter(NetworkRig rig)
        {
            if (!CanEnter(rig))
            {
                Debug.LogError("Cannot enter");
                return;
            }
            ChangeRigPresence(true, rig.Id);
        }
        public void Exit(NetworkRig rig)
        {
            ChangeRigPresence(false, rig.Id);
        }

        void ChangeRigPresence(bool present, NetworkBehaviourId rigId)
        {
            LocalChangeRigPresence(present, rigId);
            RPC_ChangeRigPresence(present, rigId);
        }

        void LocalChangeRigPresence(bool present, NetworkBehaviourId rigId)
        {
            NetworkRig rig = null;
            if(present)
            {
                if (RigsInzone.Contains(rigId) == false)
                {
                    RigsInzone.Add(rigId);
                    CheckStatus();
                    ZoneUser zoneUser = ZoneUserForRigId(rigId);
                    if (zoneUser)
                    {
                        zoneUser.DidEnterZone(this);
                        foreach (var listener in zoneListeners) listener.OnZoneUserEnterZone(zoneUser);
                    }
                }
            }
            else
            {
                if (RigsInzone.Contains(rigId))
                {
                    RigsInzone.Remove(rigId);
                    CheckStatus();
                    if (Runner.TryFindBehaviour(rigId, out rig))
                    {
                        ZoneUser zoneUser = rig.GetComponentInChildren<ZoneUser>();
                        if (zoneUser)
                        {
                            zoneUser.DidExitZone(this);
                            foreach (var listener in zoneListeners) listener.OnZoneUserExitZone(zoneUser);
                        }
                    }
                }
            }
        }

        [Rpc(InvokeLocal = false)]
        void RPC_ChangeRigPresence(NetworkBool present, NetworkBehaviourId id)
        {
            LocalChangeRigPresence(present, id);
        }

        public bool onlyRigInZoneCanLock = true;
        [ContextMenu("ToggleLock")]
        public void ToggleLock()
        {
            var rigInfo = RigInfo.FindRigInfo(Runner);
            if (onlyRigInZoneCanLock && rigInfo && rigInfo.localNetworkedRig && RigsInzone.Contains(rigInfo.localNetworkedRig) == false)
            {
                // We forbid the lock, as the local user is not in it
                // We still warn that a change has been tried, so that every listener can update according to the (non updated) status
                OnZoneChange();
                return;
            }
            ChangeForcedLock(!IsForceLocked);
        }

        [ContextMenu("Lock")]
        public void Lock()
        {
            ChangeForcedLock(true);
        }

        [ContextMenu("Unlock")]
        public void Unlock()
        {
            ChangeForcedLock(false);
        }

        public void ChangeForcedLock(bool locked)
        {
            if (RigsInzone.Count == 0 && !allowEmptyZoneLocking)
            {
                //We don't allow to lock empty zones

                // We still warn that a change has been tried, so that every listener can update according to the (non updated) status
                OnZoneChange();

                return;
            }
            LocalChangeForcedLock(locked);
            Rpc_ChangeForcedLock(locked);
        }


        public void LocalChangeForcedLock(bool locked)
        { 
            if (IsForceLocked == locked) return;
            IsForceLocked = locked;
            CheckStatus();
        }

        [Rpc(InvokeLocal = false)]
        void Rpc_ChangeForcedLock(NetworkBool locked)
        {
            LocalChangeForcedLock(locked);
        }

        /**
         * Called after a zone update
         */
        void CheckStatus()
        {
            if (RigsInzone.Count == 0 && IsForceLocked && !allowEmptyZoneLocking)
            {
                //We don't allow to lock empty zones
                IsForceLocked = false;
            }

            OnZoneChange();
        }

        void OnLockChange()
        {
            foreach (var l in zoneListeners)
            {
                if (l != null) l.OnLockChange(this);
            }
        }

        void OnZoneChange() 
        { 
            foreach(var l in zoneListeners)
            {
                if (l != null) l.OnZoneChanged(this);
            }
        }

        private void OnDestroy()
        {
            spawned = false;
            if(Runner) Runner.RemoveCallbacks(this);
            foreach (var l in zoneListeners)
            {
                if (l != null) l.OnZoneDestroyed(this);
            }
            UnregisterInZoneManager();
        }

        #region INetworkRunnerCallbacks

        public async void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            // We need someone to own the zone to handle the sync of this zone data
            // We give to the first player in the player list
            await Object.EnsureAuthorityIsAttributedAsync();

            // We check that someone did not disconnect while being in the zone
            List<NetworkBehaviourId> removedIds = new List<NetworkBehaviourId>();
            foreach (var rigId in RigsInzone)
            {
                NetworkBehaviour b;
                if (!Runner.TryFindBehaviour(rigId, out b))
                {
                    removedIds.Add(rigId);
                }
            }
            if(removedIds.Count > 0)
            {
                foreach (var rigId in removedIds) RigsInzone.Remove(rigId);
                CheckStatus();
            }
        }
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }


        public void OnInput(NetworkRunner runner, NetworkInput input) { }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

        public void OnConnectedToServer(NetworkRunner runner) { }

        public void OnDisconnectedFromServer(NetworkRunner runner) { }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

        public void OnSceneLoadDone(NetworkRunner runner) { }

        public void OnSceneLoadStart(NetworkRunner runner) { }
        #endregion
    }
}
