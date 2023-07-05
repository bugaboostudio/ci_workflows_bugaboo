using Fusion.XR.Shared.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.XR.Shared
{
    [RequireComponent(typeof(NetworkHand))]
    public class NetworkHandRay : NetworkBehaviour
    {
        [Networked(OnChanged = nameof(OnRayChange))]
        public RayData Ray { get; set; }

        NetworkHand networkHand;
        HardwareHandRay _localHardwareHandRay;
        HardwareHandRay LocalHardwareHandRay
        {
            get
            {
                if (!Object.HasInputAuthority) return null;
                if(_localHardwareHandRay == null)
                {
                    _localHardwareHandRay = networkHand.LocalHardwareHand.GetComponent<HardwareHandRay>();
                }
                return _localHardwareHandRay;
            }
        }

        [Header("Representation")]
        public LineRenderer lineRenderer;
        public float width = 0.02f;
        public Material lineMaterial;

        private void Awake()
        {
            networkHand = GetComponent<NetworkHand>();

            PrepareRay();            
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            // Note: as an alternative, it is also possible to add the ray data to leftRay and rightRay fields in the RigInput structure, and then accessing them here with GetInputs
            if (Object.HasInputAuthority)
            {
                Ray = LocalHardwareHandRay.rayDescriptor.Ray;
            }
        }

        static void OnRayChange(Changed<NetworkHandRay> changed)
        {
            // Will be called on each clients
            changed.Behaviour.UpdateRay();
        }

        void PrepareRay()
        {
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
                lineRenderer.material = lineMaterial;
                lineRenderer.numCapVertices = 4;
            }
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            lineRenderer.useWorldSpace = true;
            lineRenderer.enabled = false;
        }

        void UpdateRay()
        {
            var ray = Ray;
            if (Object.HasInputAuthority)
            {
                // We don't display the remote beam locally
                ray.isRayEnabled = false;
            }
            UpdateRay(ray);
        }

        void UpdateRay(RayData ray)
        {
            lineRenderer.enabled = ray.isRayEnabled;
            if (ray.isRayEnabled)
            {
                lineRenderer.SetPositions(new Vector3[] { ray.origin, ray.target });
                lineRenderer.positionCount = 2;
                lineRenderer.startColor = ray.color;
                lineRenderer.endColor = ray.color;
            }
        }
    }
}

