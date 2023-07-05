using Fusion.XR.Shared.Rig;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/**
 * 
 * Drive eye movements (the gazingTransforms) to find the closest GazeTarget that won't cause a to strong rotation of the eyes
 * 
 **/

namespace Fusion.XR
{
    public class Gazer : MonoBehaviour
    {
        [System.Serializable]
        public struct GazePriority
        {
            public float maxAngle;
            public float maxDistance;
            public float maxDistanceSqr;
        }

        public GazeInfo gazeInfo;
        [Header("Gazer description")]
        public Transform gazerReferencePosition;
        [Tooltip("Gaze target that should not be followed by this gazer (child GazeTarget are automatically added)")]
        public List<GazeTarget> ignoredGazeTargets = new List<GazeTarget>();
        [Tooltip("The eye transform that will be rotated")]
        public List<Transform> gazingTransforms = new List<Transform>();
        [Tooltip("If the eye transform need additional rotation offset (forward not behind a Vector.zero eurler rotation for it), it can be specified here")]
        public List<Vector3> gazingTransformOffsets = new List<Vector3>();
        [Tooltip("If true, Gaze target child of the gazer will be added to the ignored GazeTarget list")]
        public bool ignoreChildTarget = true;
        public float gazingRotationSpeed = 20;
        [Header("Targetting strategy")]
        public GazePriority mainGazePriority = new GazePriority { maxAngle = 24, maxDistance = 4.5f };
        public bool allowSwitchingToSecondaryTargets = true;
        public float maxTimeOnMainTarget = 10;
        public float maxTimeOnSecondaryTarget = 2;
        public float minTargetDistance = 0.02f;
        float minTargetDistanceSqr = 0.004f;
        [Header("Random gaze position when no currentTarget found")]
        public float maxRandomShareOfMaxAngle = 0.33f;
        public float minTimeBeforeNextRandomGaze = 3f;
        public float maxTimeBeforeNextRandomGaze = 5f;

        [Header("Target sorting results")]
        public GazeTarget currentTarget;
        public List<GazeTarget> validTargets = new List<GazeTarget>();


        [Header("Performances")]
        public float delayBetweentargetSort = 0.6f;
        [Tooltip("If defined, when the renderer associated to eyeRendererVisibility is not visible, the Gazer targetting computation won't be done")]
        public RendererVisible eyeRendererVisibility;
        public bool disableAtMainCameraDistance = true;
        public float mainCameraMaxDistance = 4;
        private float mainCameraMaxDistanceSqr = 4;
        public bool disableAtMainCameraAngle = true;
        public float mainCameraMaxAngle = 90;
        public bool disableAtMainCameraBack = true;
        NetworkObject no;
        RigInfo rigInfo;

        Camera mainCamera;

        private void Awake()
        {
            if (gazerReferencePosition == null) gazerReferencePosition = transform;
            if (ignoreChildTarget)
            {
                foreach (var t in GetComponentsInChildren<GazeTarget>())
                {
                    if (!ignoredGazeTargets.Contains(t)) ignoredGazeTargets.Add(t);
                }
            }
            mainGazePriority.maxDistanceSqr = mainGazePriority.maxDistance * mainGazePriority.maxDistance;
            minTargetDistanceSqr = minTargetDistance * minTargetDistance;
            mainCameraMaxDistanceSqr = mainCameraMaxDistance * mainCameraMaxDistance;
            no = GetComponentInParent<NetworkObject>();
        }

        private void Start()
        {
            if (gazeInfo == null)
            {
                if (no && no.Runner) gazeInfo = no.Runner.GetComponentInChildren<GazeInfo>();
            }
            if (gazeInfo == null) gazeInfo = FindObjectOfType<GazeInfo>();
        }

        float lastTargetsSort = 0;
        float timeSpentOnTarget = 0;

        GazeTarget previouslySelectedTarget = null;
        bool targettingSecondaryTarget = false;
        float maxRandomizedTimeOnMainTarget = 10;
        float maxRandomizedTimeOnSecondaryTarget = 2;

        float delayBetweenUpdateWhenNoTarget = 0.1f;
        float delayBetweenUpdateWithTarget = 0.015f;
        float lastUpdateGazer = 0;
        bool isSortingTarget = false;

        float lastForwardOffsetChange = -1;
        float nextForwardOffsertChangeDelay = 5;
        Vector3 neutralGazeDirectionOffset;

        Transform GlobalCameraPosition()
        {
            Transform globalCameraPosition = null;
            if (no && no.Runner && rigInfo == null)
            {
                rigInfo = RigInfo.FindRigInfo(no.Runner);
            }
            if (rigInfo && rigInfo.localHardwareRig)
            {
                globalCameraPosition = rigInfo.localHardwareRig.headset.transform;
            }
            if (globalCameraPosition == null && mainCamera == null) mainCamera = Camera.main;
            if (globalCameraPosition == null && mainCamera != null) globalCameraPosition = mainCamera.transform;
            return globalCameraPosition;
        }

        public void Update()
        {
            // We try not to update the eye when it is not required: eye not visisble, too far, updated recently, ...
            if (eyeRendererVisibility && eyeRendererVisibility.isVisible == false) return;
            if (currentTarget == null)
            {
                if((Time.time - lastUpdateGazer) < delayBetweenUpdateWhenNoTarget) return;
            }
            else 
            {
                if ((Time.time - lastUpdateGazer) < delayBetweenUpdateWithTarget) return;
            }

            if (!VisibleMainCAmeraPosition()) return;

            UpdateGazer();
        }



        // If needed, first sort valid observable targets, then elect the best target, and finally look in its direction
        public async void UpdateGazer() {
            lastUpdateGazer = Time.time;
            if (isSortingTarget) return;

            // We sort target if we do not have a valid target, or if we did not sort targets for some time
            bool shouldsortTarget = false;
            if (lastTargetsSort == 0 || (Time.time - lastTargetsSort) > delayBetweentargetSort) shouldsortTarget = true;
            if (currentTarget != null && !IsValidTarget(currentTarget)) shouldsortTarget = true;
            if (shouldsortTarget) await SortTargets();

            SelectTarget();
            if (currentTarget)
            {
                FollowTarget(currentTarget);
            }
            else
            {
                if(lastForwardOffsetChange == -1 || (Time.time - lastForwardOffsetChange) > nextForwardOffsertChangeDelay)
                {
                    // We have no target: we add a bit of randomness around forward to determine gazing direction, changing from time to time, to give a more natural gaze
                    lastForwardOffsetChange = Time.time;
                    nextForwardOffsertChangeDelay = Random.Range(minTimeBeforeNextRandomGaze, maxTimeBeforeNextRandomGaze);
                    var direction = Vector3.RotateTowards(gazerReferencePosition.forward, Random.insideUnitSphere, maxRandomShareOfMaxAngle * mainGazePriority.maxAngle * Mathf.Deg2Rad, 1);
                    neutralGazeDirectionOffset = direction - gazerReferencePosition.forward;
                }
                LookInDirection(gazerReferencePosition.forward + neutralGazeDirectionOffset);
            }
        }

        // Check if the camera is too far, or has a too big angle with the gazer (leading to the target not being visible)
        bool VisibleMainCAmeraPosition()
        {
            Transform globalCameraPosition = GlobalCameraPosition();
            if (disableAtMainCameraDistance)
            {
                if (globalCameraPosition && (gazerReferencePosition.position - globalCameraPosition.position).sqrMagnitude > mainCameraMaxDistanceSqr)
                {
                    return false;
                }
            }

            if (disableAtMainCameraBack)
            {
                if (globalCameraPosition && globalCameraPosition.InverseTransformPoint(gazerReferencePosition.position).z < 0)
                {
                    return false;
                }
            }

            if (disableAtMainCameraAngle)
            {
                if (globalCameraPosition && Vector3.Angle(-globalCameraPosition.transform.forward, gazerReferencePosition.forward) > mainCameraMaxAngle)
                {
                    return false;
                }
            }

            return true;
        }

        protected virtual void LookInDirection(Vector3 gazeDirection)
        {

            int i = 0;
            Quaternion gazeRotation = Quaternion.LookRotation(gazeDirection);

            foreach (var gazingTransform in gazingTransforms)
            {
                Vector3 offset = Vector3.zero;
                if (gazingTransformOffsets.Count > i)
                    offset = gazingTransformOffsets[i];

                gazingTransform.rotation = Quaternion.Slerp(gazingTransform.rotation, gazeRotation * Quaternion.Euler(offset), Time.deltaTime * gazingRotationSpeed);
                i++;
            }
        }

        protected virtual void FollowTarget(GazeTarget target)
        {

            Vector3 gazeDirection = target.transform.position - gazerReferencePosition.position;
            if (gazeDirection.sqrMagnitude < minTargetDistanceSqr)
            {
                gazeDirection = gazerReferencePosition.forward;
            }
            LookInDirection(gazeDirection);


#if UNITY_EDITOR
            foreach (var c in validTargets)
            {
                if (c == null) continue;
                if (c == target)
                {
                    if (targettingSecondaryTarget)
                    {
                        Debug.DrawRay(gazerReferencePosition.position, 0.5f * (target.transform.position - gazerReferencePosition.position), Color.yellow);
                    }
                    else
                    {
                        Debug.DrawRay(gazerReferencePosition.position, 0.5f * (target.transform.position - gazerReferencePosition.position), Color.green);
                    }
                }
                else
                {
                    Debug.DrawRay(gazerReferencePosition.position, 0.5f * (c.transform.position - gazerReferencePosition.position), Color.grey);
                }
            }
#endif
        }

        #region Gaze target sort
        [System.Serializable]
        public struct GazeTargetInfo {
            public int index;
            public Vector3 position;
        }

        // Planify in the GazeInfo a request for target sorting
        public async Task SortTargets()
        {
            isSortingTarget = true;
            currentTarget = null;

            validTargets = await gazeInfo.RequestSort(this);

            // Update random time
            maxRandomizedTimeOnMainTarget = Random.Range(0.7f * maxTimeOnMainTarget, maxTimeOnMainTarget); ;
            maxRandomizedTimeOnSecondaryTarget = Random.Range(0.7f * maxTimeOnSecondaryTarget, maxTimeOnSecondaryTarget); ;

            lastTargetsSort = Time.time;
            isSortingTarget = false;
        }
        #endregion

        void SelectTarget()
        {
            // Change from time to time between main and secondary targets
            if (allowSwitchingToSecondaryTargets && validTargets.Contains(previouslySelectedTarget))
            {
                timeSpentOnTarget += Time.deltaTime;
                if (previouslySelectedTarget == validTargets[0] && timeSpentOnTarget > maxRandomizedTimeOnMainTarget && validTargets.Count > 1)
                {
                    // We spent enough time on main target, we spent a bit of time on another valid target
                    currentTarget = validTargets[Random.Range(1, validTargets.Count - 1)];
                    timeSpentOnTarget = 0;
                    targettingSecondaryTarget = true;
                }
                else if (previouslySelectedTarget != validTargets[0] && timeSpentOnTarget > maxRandomizedTimeOnSecondaryTarget)
                {
                    // We gave a few time for a secondary target, we return on the main target
                    currentTarget = validTargets[0];
                    timeSpentOnTarget = 0;
                    targettingSecondaryTarget = false;
                }
                else
                {
                    // No need to change
                    currentTarget = previouslySelectedTarget;
                }
            }
            else if (validTargets.Count > 0)
            {
                currentTarget = validTargets[0];
                targettingSecondaryTarget = false;
            }
            
            previouslySelectedTarget = currentTarget;
        }

        public bool IsValidTarget(GazeTarget target)
        {
            var move = target.transform.position - gazerReferencePosition.position;
            var distanceSqr = move.sqrMagnitude;
            var angle = Vector3.Angle(gazerReferencePosition.transform.forward, move);
            return angle <= mainGazePriority.maxAngle && distanceSqr <= mainGazePriority.maxDistanceSqr;
        }
    }
}
