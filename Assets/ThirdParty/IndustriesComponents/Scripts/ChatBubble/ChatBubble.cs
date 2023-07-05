using Fusion.XR.Zone;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/**
 * 
 *  ChatBubble has a IZoneListener interface to register itself and get informed when a player enter or exit the zone
 *  It provides methods to update :
 *      - the zone & bubble status according to seat availability
 *      - the lock button status
 *      - the current number of players displayed on top of the zone
 *  
 **/

namespace Fusion.Samples.IndustriesComponents
{
    [RequireComponent(typeof(Zone))]
    public class ChatBubble : MonoBehaviour, IZoneListener
    {
        public TextMeshPro numberOfUserTMP;
        public Material lockMaterial;
        private Material defaultMaterial;

        [SerializeField]
        private MeshRenderer domeMeshRenderer;
        [SerializeField]
        private MeshRenderer domeIndideMeshRenderer;
        [SerializeField]
        private MeshRenderer ringMeshRenderer;

        public Touchable lockButton;
        public GameObject lockStand;

        public Renderer[] renderers;
        public Renderer[] domeRenderers;
        public Collider[] colliders;
        public TMP_Text[] texts;
        public Transform animatedZone;
        public Transform domeAnimatedZone;

        bool displayDomeIfNotLocked = true;

        Zone zone;
        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
            domeRenderers = domeAnimatedZone.GetComponentsInChildren<Renderer>();
            colliders = GetComponentsInChildren<Collider>();
            texts = GetComponentsInChildren<TMP_Text>();
            if (domeMeshRenderer) defaultMaterial = domeMeshRenderer.material;
            else if (ringMeshRenderer) defaultMaterial = ringMeshRenderer.material;

            // register to get informed when a player enter or exit the bubble 
            zone = GetComponent<Zone>();
            zone.RegisterListener(this);

            var dynamicZone = GetComponent<DynamicZone>();
            if (dynamicZone) displayDomeIfNotLocked = dynamicZone.displayDomeIfNotLocked;

        }

        private void OnDestroy()
        {
            zone.UnregisterListener(this);
        }

        #region IZoneListener
        // Update bubble status after a lock/unlock 
        public void OnLockChange(Zone zone)
        {
            UpdateLockDisplay();
        }

        // Update zone status when a player join/left the zone or when the zone has been locked/unlocked
        public void OnZoneChanged(Zone zone)
        {
            UpdateLockDisplay();
            UpdateUsersInChatBubbleDisplay();
            UpdateLockButton();
        }

        public void OnZoneDestroyed(Zone zone) {}
        public void OnZoneUserEnterZone(ZoneUser zoneUser) { }
        public void OnZoneUserExitZone(ZoneUser zoneUSer) { }

        // Change the zone visibility without animation
        public void InstantChangeZoneVisibility(Zone zone, bool visible)
        {
            foreach (var r in renderers) r.enabled = visible;
            foreach (var c in colliders) c.enabled = visible;
            foreach (var t in texts) t.enabled = visible;
            if (lockStand) lockStand.gameObject.SetActive(visible);
            animatedScale = visible ? 1 : 0;
            if (!displayDomeIfNotLocked && zone.CanAcceptNewRig)
            {
                foreach (var r in domeRenderers) r.enabled = false;
            }
        }

        float animatedScale = 1;
        float animatedScaleTarget = 1;

        // ChangeZoneVisibility is in charge of animating the zone when it should appear or disappear
        public IEnumerator ChangeZoneVisibility(Zone zone, bool visible)
        {
            float currentRequestTarget = visible ? 1 : 0;
            animatedScaleTarget = currentRequestTarget;

            // Check the zone is initialized
            if (animatedZone)
            {
                if (lockStand) lockStand.gameObject.SetActive(false);
                foreach (var c in colliders) c.enabled = false;
                foreach (var t in texts) t.enabled = false;

                // check if the zone should be visible or not
                if (visible)
                {
                    // zone should be visible
                    // rescale the zone to zero
                    animatedZone.localScale = Vector3.zero;

                    // enable all zone renderers
                    foreach (var r in renderers) r.enabled = true;

                    // Check if dome must be visible
                    if (!displayDomeIfNotLocked && zone.CanAcceptNewRig)
                    {
                        foreach (var r in domeRenderers) r.enabled = false;
                    }

                    // Animation to rescale the dome to 1
                    while (animatedScale < 1 && animatedScaleTarget == 1)
                    {
                        animatedZone.localScale = animatedScale * Vector3.one;
                        animatedScale = Mathf.Min(1, step + animatedScale);
                        yield return new WaitForSeconds(animationDuration * step);
                    }
                    if (animatedScaleTarget == 1) animatedZone.localScale = Vector3.one;
                }
                else
                {
                    // dome should not be visible
                    // rescale the dome to 1
                    animatedZone.localScale = Vector3.one;

                    // enable all dome renderers
                    foreach (var r in renderers) r.enabled = true;

                    // Check if dome must be visible
                    if (!displayDomeIfNotLocked && zone.CanAcceptNewRig && domeAnimatedScale == 0)
                    {
                        foreach (var r in domeRenderers) r.enabled = false;
                    }

                    // Animation to rescale the dome to 0
                    while (animatedScale > 0 && animatedScaleTarget == 0)
                    {
                        animatedZone.localScale = animatedScale * Vector3.one;
                        animatedScale = Mathf.Max(0, animatedScale - step);
                        yield return new WaitForSeconds(animationDuration * step);
                    }
                    if (animatedScaleTarget == 1) animatedZone.localScale = Vector3.zero;
                }
            }
            if (animatedScaleTarget == currentRequestTarget) InstantChangeZoneVisibility(zone, visible);
        }

        float domeAnimatedScale = 0;
        float domeAnimatedScaleTarget = 0;
        const float step = 0.01f;
        const float animationDuration = 0.1f;
        bool domeVisible = false;

        // ChangeDomeVisibility is in charge of animating the dome when it should appear or disappear
        public IEnumerator ChangeDomeVisibility(bool visible)
        {
            if (displayDomeIfNotLocked) yield break;
            domeVisible = visible;

            // Compute the final scale size
            float currentRequestTarget = visible ? 1 : 0;
            domeAnimatedScaleTarget = currentRequestTarget;

            // Check the dome is initialized
            if (domeAnimatedZone)
            {
                // check if the dome should be visible or not
                if (visible)
                {
                    // dome should be visible
                    // rescale the dome to zero
                    domeAnimatedZone.localScale = Vector3.zero;
                    // enable all dome renderers
                    foreach (var r in domeRenderers) r.enabled = true;

                    // Animation to rescale the dome to 1
                    while (domeAnimatedScale < 1 && domeAnimatedScaleTarget == 1)
                    {
                        domeAnimatedZone.localScale = domeAnimatedScale * Vector3.one;
                        domeAnimatedScale = Mathf.Min(1, step + domeAnimatedScale);
                        yield return new WaitForSeconds(animationDuration * step);
                    }
                    if (domeAnimatedScaleTarget == 1) domeAnimatedZone.localScale = Vector3.one;
                }
                else
                {
                    // dome should not be visible
                    // rescale the dome to 1
                    domeAnimatedZone.localScale = Vector3.one;
                    // enable all dome renderers
                    foreach (var r in domeRenderers) r.enabled = true;

                    // Animation to rescale the dome to 0
                    while (domeAnimatedScale > 0 && domeAnimatedScaleTarget == 0)
                    {
                        domeAnimatedZone.localScale = domeAnimatedScale * Vector3.one;
                        domeAnimatedScale = Mathf.Max(0, domeAnimatedScale - step);
                        yield return new WaitForSeconds(animationDuration * step);
                    }
                    if (domeAnimatedScaleTarget == 0) domeAnimatedZone.localScale = Vector3.zero;
                }
            }
        }

        public void DidEndChangeZoneVisibility(Zone zone, bool visible) { }
        #endregion

        // UpdateLockDisplay update the dome meshes materials according to seat availability in the zone
        private void UpdateLockDisplay()
        {
            // Check if the zone is full
            if (!zone.CanAcceptNewRig)
            {
                // The zone is full 
                // Update mesh renderers with lock material
                if (domeMeshRenderer) domeMeshRenderer.material = lockMaterial;
                if (domeIndideMeshRenderer) domeIndideMeshRenderer.material = lockMaterial;
                if (ringMeshRenderer) ringMeshRenderer.material = lockMaterial;

                // check if dome should be displayed
                if (!displayDomeIfNotLocked)
                {
                    // Launch the animation
                    StartCoroutine(ChangeDomeVisibility(true));
                }
            }
            else
            {
                // The zone can accept player
                // restore defaults material
                if (domeMeshRenderer) domeMeshRenderer.material = defaultMaterial;
                if (domeIndideMeshRenderer) domeIndideMeshRenderer.material = defaultMaterial;
                if (ringMeshRenderer) ringMeshRenderer.material = defaultMaterial;

                // check if dome should be displayed
                if (!displayDomeIfNotLocked)
                {
                    // Launch the animation
                    StartCoroutine(ChangeDomeVisibility(false));
                }
            }
        }

        // Update the number of players on the display
        private void UpdateUsersInChatBubbleDisplay()
        {
            if (numberOfUserTMP) numberOfUserTMP.text = zone.RigsInzone.Count + " / " + zone.maxCapacity;
        }

        // UpdateLockButton is in charge to update the 3D lock button status with the zone status
        void UpdateLockButton()
        {
            // If the logic desactivated the button, or if a remote user pressed it, we make the button representation match
            if (lockButton != null && zone.IsForceLocked != lockButton.buttonStatus)
            {
                lockButton.SetButtonStatus(zone.IsForceLocked);
            }
        }
    }
}
