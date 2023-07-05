using Fusion;
using Fusion.Samples.Expo;
using Fusion.Sockets;
using Fusion.XR.Shared.Grabbing;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Fusion.XR.Tools
{

    [System.Serializable]
    public struct DrawPoint : INetworkStruct
    {
        public const int RepresentationSize = 4;
        public NetworkBool isNewLineOrigin;
        public Vector3 localPosition;
        public float drawPressure;// A draw pressure of 0 marks the end of a line
    }

    /**
     * Class representing a 3D drawing
     * DrawPoints are stored in a networked var, and once its capacity reached, we can create new Draw object that will follow the first one * 
     */
    [OrderAfter(typeof(NetworkRigidbody), typeof(NetworkTransform), typeof(NetworkGrabbable))]
    public class Draw : NetworkBehaviour, INetworkRunnerCallbacks
    {
        public ExpoManagers managers;

        public LineRenderer linePrefab;

        public LineRenderer currentLine;
        public List<LineRenderer> lines = new List<LineRenderer>();

        public List<Vector3> drawingPoints = new List<Vector3>();
        public List<float> drawingPressures = new List<float>();
        public List<float> drawingPathLength = new List<float>();
        public Vector3 lastPoint;
        NetworkTransform networkTransform;

        public MeshRenderer handleRenderer;
        public Collider handleCollider;
        [Networked(OnChanged = nameof(OnBaseDrawChanged))]
        public Draw BaseDraw { get; set; }

        // 2D boards
        public bool isBoardDrawing = false;

        [Networked]
        public int PointCount { get; set; }

        int lastDrawnPointIndex = 0;
        [Networked(OnChanged = nameof(OnDrawPointsChanged)), Capacity(500)]// Max possible capacity here: 700
        public NetworkArray<DrawPoint> DrawPoints { get; }

        static void OnDrawPointsChanged(Changed<Draw> changed)
        {
            changed.Behaviour.DrawLatestPoints();
        }

        static void OnBaseDrawChanged(Changed<Draw> changed)
        {
            // We disable the handle if we are following a base draw
            changed.Behaviour.handleRenderer.enabled = changed.Behaviour.BaseDraw == null;
            changed.Behaviour.handleCollider.enabled = changed.Behaviour.BaseDraw == null;
        }

        void UpdateHandle()
        {
            handleRenderer.enabled = BaseDraw == null;
            handleCollider.enabled = BaseDraw == null;
        }

        void DrawLatestPoints()
        {
            while (lastDrawnPointIndex < PointCount)
            {
                LocalAddDrawingPoint(DrawPoints[lastDrawnPointIndex]);
                lastDrawnPointIndex++;
            }
        }

        public Vector3 LocalAddDrawingPoint(DrawPoint newPoint)
        {
            if (currentLine == null)
            {
                CreateLine();
            }
            var drawingPosition = currentLine.transform.TransformPoint(newPoint.localPosition);
            return LocalAddDrawingPoint(drawingPosition: drawingPosition, drawPRessure: newPoint.drawPressure);
        }

        private void Awake()
        {
            networkTransform = GetComponent<NetworkTransform>();
            if (handleCollider == null) handleCollider = GetComponentInChildren<Collider>();
            if (handleRenderer == null) handleRenderer = GetComponentInChildren<MeshRenderer>();
        }

        public override void Spawned()
        {
            base.Spawned();
            Runner.AddCallbacks(this);
            if (managers == null) managers = (ExpoManagers)ExpoManagers.FindInstance(Object.Runner);
            if (managers == null) Debug.LogError("ExpoManagers not found");
        }

        private void OnDestroy()
        {
            if (Runner) Runner.RemoveCallbacks(this);
        }

        public void PauseDrawing()
        {
            AddLineEnd();
        }

        public void ResumeDrawing() { }

        public void AddLineEnd()
        {
            AddDrawingPoint(Vector3.zero, 0); // Separator Point
        }

        public bool AddDrawingPoint(Vector3 drawingPosition, float drawPRessure)
        {
            if (PointCount >= (DrawPoints.Length - 1) && drawPRessure != 0)
            {
                // We're reaching the max number of points, and we want to keep one point to warn of line end
                return false;
            }

            if (PointCount >= DrawPoints.Length)
            {
                // Max number of points reached
                return false;
            }
            var localDrawingPosition = Vector3.zero;
            if (drawPRessure != 0 && currentLine == null)
            {
                CreateLine();
            }
            if (currentLine != null)
            {
                localDrawingPosition = currentLine.transform.InverseTransformPoint(drawingPosition);
            }

            DrawPoints.Set(PointCount, new DrawPoint() { localPosition = localDrawingPosition, drawPressure = drawPRessure });
            PointCount++;
            DrawLatestPoints();
            return true;
        }

        public Vector3 LocalAddDrawingPoint(Vector3 drawingPosition, float drawPRessure)
        {
            Vector3 localDrawingPosition = Vector3.zero;

            if (drawPRessure == 0)
            {
                // Separation point (not displayed)
                currentLine = null;
            }
            else
            {
                // Regular point
                if (currentLine == null)
                {
                    CreateLine();
                }

                localDrawingPosition = currentLine.transform.InverseTransformPoint(drawingPosition);

                drawingPoints.Add(localDrawingPosition);
                drawingPressures.Add(drawPRessure);
                currentLine.positionCount = drawingPoints.Count;
                currentLine.SetPositions(drawingPoints.ToArray());
                if (drawingPathLength.Count > 0)
                {
                    var lastLength = drawingPathLength[drawingPathLength.Count - 1];
                    var total = lastLength + Vector3.Distance(lastPoint, localDrawingPosition);
                    drawingPathLength.Add(total);
                    AnimationCurve widthCurve = new AnimationCurve();
                    int index = 0;
                    foreach (var length in drawingPathLength)
                    {
                        if (index < drawingPressures.Count)
                        {
                            widthCurve.AddKey(length / total, drawingPressures[index]); ;
                        }
                        index++;

                    }
                    currentLine.widthCurve = widthCurve;
                }
                else
                {
                    drawingPathLength.Add(0);
                }
                lastPoint = localDrawingPosition;
            }

            if (isBoardDrawing) managers.boardManager.ActivateBoards();

            return localDrawingPosition;
        }

        Vector3 lastRecordedPosition;
        float minMovementSqr = 0.001f * 0.001f;

        private void Update()
        {
            if (isBoardDrawing)
            {
                Vector3 interpolatedPosition = transform.position;
                if (networkTransform) interpolatedPosition = networkTransform.InterpolationTarget.position;
                // If the drawing has been moved, we should restart recording boards
                if ((interpolatedPosition - lastRecordedPosition).sqrMagnitude > minMovementSqr)
                {
                    managers.boardManager.ActivateBoards();
                    lastRecordedPosition = interpolatedPosition;
                }
            }

            // If we had to split the drawing into several part, the other parts follow the base one
            if (BaseDraw != null)
            {
                transform.position = BaseDraw.transform.position;
                transform.rotation = BaseDraw.transform.rotation;
            }
        }

        public override void Render()
        {
            base.Render();
            // If we had to split the drawing into several part, the other parts follow the base one (extrapolation here). It requires the OrderAfter directive at the start of the class
            //  to override the behavior of NetworkTransform or NetworkRigobody components
            if (BaseDraw != null)
            {
                networkTransform.InterpolationTarget.transform.position = BaseDraw.networkTransform.InterpolationTarget.transform.position;
                networkTransform.InterpolationTarget.transform.rotation = BaseDraw.networkTransform.InterpolationTarget.transform.rotation;
            }
        }

        void CreateLine()
        {
            drawingPoints.Clear();
            drawingPressures.Clear();
            drawingPathLength.Clear();
            currentLine = GameObject.Instantiate(linePrefab, transform.position, transform.rotation);
            var lineParentTransform = transform;
            if (networkTransform) lineParentTransform = networkTransform.InterpolationTarget;
            currentLine.transform.SetParent(lineParentTransform);
            currentLine.transform.position = transform.position;
            currentLine.transform.rotation = transform.rotation;
            lines.Add(currentLine);
        }

        #region INetworkRunnerCallbacks 
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            // Ensure we have an owner to have someone able to send point to user joining later the session
            Object.EnsureAuthorityIsAttributed();
        }
        #endregion

        #region INetworkRunnerCallbacks (unused)
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
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
        #endregion
    }
}
