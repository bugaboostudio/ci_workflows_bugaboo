using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;
using Fusion.XR.Shared.Grabbing;
using Fusion.XR.Shared.Rig;

namespace Fusion.XR.Tools
{
    /**
     * Pen-like drawer that generate 3D line renderer in space
     * Create drawPrefab objects containing Draw when the drawer is grabbed, and add point to the Draw when the associated input action is pressed
     *  
     * If a projection board is set, the proximity with the board is used instead of the input action to trigger point creation
     */
    [OrderAfter(typeof(NetworkHand), typeof(NetworkRig))]
    public class Drawer : NetworkBehaviour
    {
        public Transform tip;
        public Draw drawPrefab;
        Draw currentDraw = null;

        public NetworkGrabbable grabbable;
        public InputActionProperty leftUseAction;
        public InputActionProperty rightUseAction;

        [Header("Haptic feedback")]
        public float defaultHapticAmplitude = 0.2f;
        public float defaultHapticDuration = 0.05f;

        InputActionProperty UseAction => grabbable != null && grabbable.IsGrabbed && grabbable.CurrentGrabber.hand.side == RigPart.LeftController ? leftUseAction : rightUseAction;
        public bool IsGrabbed => grabbable.IsGrabbed;
        public bool IsGrabbedByLocalPLayer => IsGrabbed && grabbable.CurrentGrabber.Object.StateAuthority == Runner.LocalPlayer;

        public bool IsUsed
        {
            get
            {
                if (projectionBoard != null)
                {
                    var coordinate = projectionBoard.InverseTransformPoint(tip.position).z;

                    return coordinate > 0 ? coordinate < boardPositiveProximityThreshold : coordinate >= boardNegativeProximityThreshold;
                }
                else
                {
                    return UseAction.action.ReadValue<float>() > minAction;

                }
            }
        }

        public float Pressure
        {
            get
            {
                if (projectionBoard != null)
                {
                    var coordinate = projectionBoard.InverseTransformPoint(tip.position).z;
                    return coordinate > 0 ? 1f : Mathf.Abs((boardNegativeProximityThreshold - coordinate) / boardNegativeProximityThreshold);
                }
                else
                {
                    return UseAction.action.ReadValue<float>();

                }
            }
        }

        [Header("Draw point detection")]
        public float drawingMinResolution = 0.001f;
        float minAction = 0.05f;
        float minDrawnPressure = 0.2f;//TODO fix pressure code (create glitchy artefact on render texture)
        Vector3 lastDrawingPosition;

        [Header("Projection board")]
        public Transform projectionBoard;
        public float boardPositiveProximityThreshold = 0.045f;
        public float boardNegativeProximityThreshold = -0.02f;

        public enum Status
        {
            NotGrabbed,
            DrawingPaused,
            Drawing
        }

        public Status status = Status.NotGrabbed;


        private void Awake()
        {
            grabbable = GetComponent<NetworkGrabbable>();
            leftUseAction.action.Enable();
            rightUseAction.action.Enable();
        }

        void SendHapticFeedback()
        {
            if (grabbable == null) return;
            if (!IsGrabbedByLocalPLayer || grabbable.CurrentGrabber.hand.LocalHardwareHand == null) return;

            grabbable.CurrentGrabber.hand.LocalHardwareHand.SendHapticImpulse(amplitude: defaultHapticAmplitude, duration: defaultHapticDuration);
        }

        public void LateUpdate()
        {
            if (!IsGrabbedByLocalPLayer)
            {
                return;
            }

            if (IsUsed)
            {
                OnDrawing();
            }
        }

        public override void FixedUpdateNetwork()
        {
            UpdateDrawerStatus();
        }

        void UpdateDrawerStatus()
        {
            if (!IsGrabbedByLocalPLayer)
            {
                if (status == Status.Drawing)
                {
                    PauseDrawing();
                }
                if (status != Status.NotGrabbed)
                {
                    FinishDraw();
                    status = Status.NotGrabbed;
                }
                return;
            }

            if (IsUsed)
            {
                if (status != Status.Drawing)
                {
                    status = Status.Drawing;
                    ResumeDrawing();
                }
            }
            else
            {
                if (status == Status.Drawing)
                {
                    status = Status.DrawingPaused;
                    PauseDrawing();
                }
            }
        }

        void CreateDraw(Vector3 position, Quaternion rotation)
        {
            if (!IsGrabbedByLocalPLayer)
            {
                Debug.LogError("Draw should be spawned by drawer");
                return;
            }
            currentDraw = Object.Runner.Spawn(drawPrefab, position, rotation, Runner.LocalPlayer);
        }

        void OnDrawing()
        {
            float drawPressure = minDrawnPressure + (1f - minDrawnPressure) * Mathf.Clamp01(Pressure);
            Vector3 drawingPosition = tip.position;
            if (Vector3.Distance(lastDrawingPosition, drawingPosition) > drawingMinResolution)
            {
                lastDrawingPosition = drawingPosition;
                if (currentDraw)
                {
                    bool drawingPossible = currentDraw.AddDrawingPoint(drawingPosition, drawPressure);
                    if (!drawingPossible)
                    {
                        // We reach the max size of a drawing: switching to a new one
                        bool previousPointAvailable = false;
                        Vector3 previousPointPosition = Vector3.zero;
                        float previousPressure = 0;
                        Vector3 baseDrawPosition = currentDraw.transform.position;
                        Quaternion baseDrawRotation = currentDraw.transform.rotation;
                        if (currentDraw.currentLine)
                        {
                            previousPointAvailable = true;
                            var lastPoint = currentDraw.DrawPoints[currentDraw.PointCount - 1];
                            previousPointPosition = currentDraw.currentLine.transform.TransformPoint(lastPoint.localPosition);
                            previousPressure = lastPoint.drawPressure;
                        }
                        var baseDraw = currentDraw;
                        PauseDrawing();
                        FinishDraw();
                        CreateDraw(baseDrawPosition, baseDrawRotation);
                        currentDraw.BaseDraw = baseDraw;
                        if(previousPointAvailable) currentDraw.AddDrawingPoint(previousPointPosition, previousPressure);
                        currentDraw.AddDrawingPoint(drawingPosition, drawPressure);
                    }
                }
                SendHapticFeedback();
            }
        }

        void PauseDrawing()
        {
            if (currentDraw != null)
            {
                currentDraw.PauseDrawing();
            }
        }

        void ResumeDrawing()
        {
            if (currentDraw == null)
            {
                CreateDraw(tip.position - 0.2f * Vector3.up, Quaternion.identity);
            }
            if (currentDraw) currentDraw.ResumeDrawing();
        }

        void FinishDraw()
        {
            if (currentDraw != null && currentDraw.lines.Count == 0)
            {
                Object.Runner.Despawn(currentDraw.Object);
            }
            currentDraw = null;
        }
    }
}
