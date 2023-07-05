using Fusion.XR.Shared.Rig;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fusion.XR.Shared.Interaction
{
    public class BeamToucher : MonoBehaviour
    {
        public bool touchUITouchButton = true;
        public bool touchTouchable = false;
        public bool touchTouchableSlider = true;
        public HardwareHand hardwareHand;

        public interface IBeamHoverListener
        {
            void OnHoverStart(BeamToucher beamToucher);
            void OnHoverEnd(BeamToucher beamToucher);
            void OnHoverRelease(BeamToucher beamToucher);
        }

        public enum TouchMode
        {
            WasPressedThisFrame,
            WasReleasedThisFrame
        }
        public TouchMode touchMode = TouchMode.WasPressedThisFrame;

        RayBeamer beamer;

        Collider hitCollider;
        IBeamHoverListener hoverListener;
        // Cache attributes, to limit the number of GetComponent calls
        UITouchButton uiTouchButton;
        Touchable touchable;
        TouchableSlider slider;
        bool noUITouchable = false;
        bool noTouchable = false;
        bool noSlider = false;

        public InputActionProperty useAction;

        private void Awake()
        {
            hardwareHand = GetComponentInParent<HardwareHand>();
            beamer = GetComponentInChildren<RayBeamer>();
            beamer.onHitEnter.AddListener(OnHitEnter);
            beamer.onHitExit.AddListener(OnHitExit);
            beamer.onRelease.AddListener(OnRelease);
        }

        private void Start()
        {
            useAction.EnableWithDefaultXRBindings(side: beamer.hand.side, new List<string> { "trigger" });
        }

        private void OnRelease(Collider collider, Vector3 hitPoint)
        {
            if(hoverListener != null)
            {
                hoverListener.OnHoverRelease(this);
            }
            ResetColliderInfo();
        }

        private void OnHitExit(Collider collider, Vector3 hitPoint)
        {
            ResetColliderInfo();
        }

        private void OnHitEnter(Collider collider, Vector3 hitPoint)
        {
            if (hitCollider != collider)
            {
                ResetColliderInfo();
            }
            hitCollider = collider;
            hoverListener = hitCollider.GetComponentInChildren<IBeamHoverListener>();
            if (hoverListener != null)
            {
                hoverListener.OnHoverStart(this);
            }
        }

        void ResetColliderInfo()
        {
            hitCollider = null;
            uiTouchButton = null;
            touchable = null;
            slider = null;
            noUITouchable = false;
            noTouchable = false;
            noSlider = false;
            if (hoverListener != null)
            {
                hoverListener.OnHoverEnd(this);
                hoverListener = null;
            }
        }

        private void Update()
        {
            if (!hitCollider) return;
            bool used = touchMode == TouchMode.WasPressedThisFrame ? useAction.action.WasPressedThisFrame() : useAction.action.WasReleasedThisFrame();
            if (used)
            {
                if (touchTouchable && !noTouchable)
                {
                    if (!uiTouchButton) uiTouchButton = hitCollider.GetComponentInParent<UITouchButton>();
                    

                    if (!uiTouchButton)
                    {
                        if (!touchable) touchable = hitCollider.GetComponentInParent<Touchable>();

                        if (touchable)
                            touchable.OnTouch();
                        else
                            noTouchable = true;
                    }
                }

                if (touchUITouchButton && !noUITouchable)
                {
                    if (!uiTouchButton) uiTouchButton = hitCollider.GetComponentInParent<UITouchButton>();
                    if (uiTouchButton)
                        uiTouchButton.touchable.OnTouch();
                    else
                        noUITouchable = true;
                }
            }

            if (!noSlider && useAction.action.IsPressed())
            {
                if (touchTouchableSlider)
                {
                    if (!slider) slider = hitCollider.GetComponentInParent<TouchableSlider>();
                    if (slider)
                        slider.MoveSliderToPosition(beamer.lastHit);
                    else
                        noSlider = true;
                }
            }
        }
    }
}