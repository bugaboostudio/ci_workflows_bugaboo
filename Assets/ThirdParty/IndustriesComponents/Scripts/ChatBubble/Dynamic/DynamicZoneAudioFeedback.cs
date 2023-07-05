using Fusion;
using Fusion.XR;
using Fusion.XR.Shared.Rig;
using Fusion.XR.Zone;
using System.Collections;
using UnityEngine;

/**
* 
* DynamicZoneAudioFeedback provides audio feedbacks when a user enter or leave a dynamic bubble
* 
* 
**/
namespace Fusion.Samples.IndustriesComponents
{
    public class DynamicZoneAudioFeedback : SimulationBehaviour, IZoneListener
    {
        public RigInfo rigInfo;
        private Zone zone;
        private SoundManager soundManager;
        private int numberOfUsersInZone = 0;
        private bool localUserWasInZone = false;

        private void Awake()
        {
            // register to get informed when a player enter or exit the bubble 
            zone = GetComponent<Zone>();
            zone.RegisterListener(this);

            if (soundManager == null) soundManager = SoundManager.FindInstance();
        }

        #region IZoneListener
        // OnZoneChanged is called when an user enter or exit the zone
        // A join sound is played when the user or a remote user joins the zone
        // An exit sound is played when the user or a remote user exit the zone
        public void OnZoneChanged(Zone zone)
        {
            if (!rigInfo)
                rigInfo = RigInfo.FindRigInfo(Runner);



            if (rigInfo)
            {   // Check if local user was in the zone

                if (rigInfo.localNetworkedRig == null) return;

                if (!localUserWasInZone)
                {
                    // local user was not is zone
                    // Check if local user is in the zone now
                    if (zone.RigsInzone.Count > 0 && zone.RigsInzone.Contains(rigInfo.localNetworkedRig))
                    {
                        localUserWasInZone = true;
                        numberOfUsersInZone = zone.RigsInzone.Count;
                        // local user enter into the zone
                        // Play enter sound only if there are more that two players in zone because an enter sound will be played when the second player will join the one created by the first user
                        if (numberOfUsersInZone > 1)
                        {
                            soundManager.PlayOneShot("PlayerJoinDynamicChatBubble");
                        }
                    }
                }
                else
                {   // local user was in the zone
                    // Check if local user is still in the zone now
                    if (zone.RigsInzone.Count > 0 && zone.RigsInzone.Contains(rigInfo.localNetworkedRig))
                    {   // local user is still into the zone
                        // Check if a player enter or exit the zone
                        if (numberOfUsersInZone < zone.RigsInzone.Count)
                        {
                            // A remote user enter the zone
                            soundManager.PlayOneShot("PlayerJoinDynamicChatBubble");
                        }
                        else if (numberOfUsersInZone > zone.RigsInzone.Count)
                        {
                            // A remote user exit the zone
                            // Play exit sound only if there are more that two players in zone because an exit sound will be played when the local player is removed of the zone because he is the last player 
                            if (zone.RigsInzone.Count > 1)
                            {
                                soundManager.PlayOneShot("PlayerLeftDynamicChatBubble");
                            }
                        }
                        numberOfUsersInZone = zone.RigsInzone.Count;
                    }
                    else
                    {
                        // local user exit the zone
                        localUserWasInZone = false;
                        soundManager.PlayOneShot("PlayerLeftDynamicChatBubble");
                    }
                }
            }
            else
                Debug.LogError("DynamicZoneAudioFeedback : rigInfo not found !");

        }

        public IEnumerator ChangeZoneVisibility(Zone zone, bool visible)
        {
            yield return null;
        }

        public void InstantChangeZoneVisibility(Zone zone, bool visible) { }
        public void DidEndChangeZoneVisibility(Zone zone, bool visible) {}
        public void OnZoneUserEnterZone(ZoneUser zoneUser) { }
        public void OnZoneUserExitZone(ZoneUser zoneUSer) { }
        public void OnLockChange(Zone zone) { }
        public void OnZoneDestroyed(Zone zone) { }
        #endregion
    }
}
