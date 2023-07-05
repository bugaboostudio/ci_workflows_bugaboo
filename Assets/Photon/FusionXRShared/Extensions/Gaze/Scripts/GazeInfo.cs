using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Fusion.XR
{
    /**
     * 
     * Store the gaze target and allow the gazers to find them quickly
     *
     * Should be stored under the hierarchy of the runner
     * 
     **/

    public class GazeInfo : MonoBehaviour
    {
        public List<GazeTarget> gazeTargets = new List<GazeTarget>();

        [Header("Sort gazers task info")]
        public float delayBetweenBatchSort = 0.6f;
        public bool debugSort = false;
        public float lastSortEnd = -1;
        public bool sorting = false;

        public void RegisterGazeTarget(GazeTarget gazeTarget)
        {
            gazeTargets.Add(gazeTarget);
        }
        public void UnregisterGazeTarget(GazeTarget gazeTarget)
        {
            gazeTargets.Remove(gazeTarget);
        }

        #region Sort
        public struct GazeTargetInfo
        {
            public int index;
            public Vector3 position;
        }

        List<Gazer> gazerRequestingSort = new List<Gazer>();
        Dictionary<Gazer, List<GazeTarget>> results = new Dictionary<Gazer, List<GazeTarget>>();
        public async Task<List<GazeTarget>> RequestSort(Gazer gazer)
        {
            gazerRequestingSort.Add(gazer);
            while (results.ContainsKey(gazer) == false)
            {
                if (sorting == false && (lastSortEnd == -1 || (Time.time - lastSortEnd) > delayBetweenBatchSort))
                {
                    await SortGazers();
                }
                else
                {
                    await Task.Delay(10);
                }
            }
            var result = results[gazer];
            results.Remove(gazer);
            return result;
        }

        public struct GazerInfo
        {
            public Vector3 gazerPosition;
            public Vector3 gazerForward;
            public float maxAngle;
            public float maxDistanceSqr;
            public List<int> ignoredTargets;
            public int index;
        }


        // Prepare data to realize batched sorting, and then starts a thread to handle the task
        //
        // TODO move to Unity Job system, instead of spawning a thread
        async Task SortGazers()
        {
            float startTime = Time.realtimeSinceStartup;
            sorting = true;
            results = new Dictionary<Gazer, List<GazeTarget>>();
            Dictionary<GazeTarget, int> gazeTargetIndex = new Dictionary<GazeTarget, int>();
            int i = 0;
            foreach (var t in gazeTargets)
            {
                gazeTargetIndex.Add(t, i);
                i++;
            }

            List<GazeTargetInfo> sortableGazeTargetInfos = new List<GazeTargetInfo>();
            Dictionary<int, GazeTarget> gazeTargetByIndexes = new Dictionary<int, GazeTarget>();
            i = 0;
            foreach (var t in gazeTargets)
            {
                if (t.isActiveAndEnabled == false) continue;
                sortableGazeTargetInfos.Add(new GazeTargetInfo { index = i, position = t.transform.position });
                gazeTargetByIndexes[i] = t;
                i++;
            }

            List<GazerInfo> gazerInfos = new List<GazerInfo>();
            // We purge any Gazer which may have been destroyed before receiving answer to its sort request
            gazerRequestingSort.RemoveAll(g => g == null);
            i = 0;
            Dictionary<Gazer, int> gazerIndexByGazer = new Dictionary<Gazer, int>();
            foreach (var gazer in gazerRequestingSort)
            {
                var ignoredTargets = new List<int>();
                foreach (var ignored in gazer.ignoredGazeTargets)
                {
                    if (gazeTargetIndex.ContainsKey(ignored))
                    {
                        ignoredTargets.Add(gazeTargetIndex[ignored]);
                    }
                }
                gazerInfos.Add(new GazerInfo
                {
                    gazerPosition = gazer.gazerReferencePosition.position,
                    gazerForward = gazer.gazerReferencePosition.transform.forward,
                    maxAngle = gazer.mainGazePriority.maxAngle,
                    maxDistanceSqr = gazer.mainGazePriority.maxDistanceSqr,
                    ignoredTargets = ignoredTargets,
                    index = i
                });
                gazerIndexByGazer[gazer] = i;
                i++;
            }

            var sortingTask = new ThreadedTask<Dictionary<int, List<GazeTargetInfo>>>(() => {
                Dictionary<int, List<GazeTargetInfo>> res = new Dictionary<int, List<GazeTargetInfo>>();
                foreach (var gazerInfo in gazerInfos)
                {
                    var sortedTargets = new List<GazeTargetInfo>();
                    float[] sqrDistances = new float[sortableGazeTargetInfos.Count];
                    foreach (var t in sortableGazeTargetInfos)
                    {
                        // Check if is ignored
                        if (gazerInfo.ignoredTargets.Contains(t.index)) continue;
                        //Check distance
                        sqrDistances[t.index] = (t.position - gazerInfo.gazerPosition).sqrMagnitude;
                        if (sqrDistances[t.index] > gazerInfo.maxDistanceSqr) continue;
                        //Check angle
                        float angle = Vector3.Angle(gazerInfo.gazerForward, (t.position - gazerInfo.gazerPosition));
                        if (angle > gazerInfo.maxAngle) continue;

                        sortedTargets.Add(t);
                    }
                    sortedTargets.Sort((GazeTargetInfo a, GazeTargetInfo b) => {
                        if (sqrDistances[a.index] < sqrDistances[b.index]) return -1;
                        if (sqrDistances[a.index] > sqrDistances[b.index]) return 1;
                        return 0;
                    });
                    res[gazerInfo.index] = sortedTargets;
                }
                return res;

            });
            var gazeTargetInfosByGazerIndex = await sortingTask.WaitOutput();

            foreach (var gazer in gazerRequestingSort)
            {
                if (gazer == null)
                {
                    if(debugSort) Debug.LogError("Gazer destroyed before receiving answer");
                    continue;
                }
                if (gazerIndexByGazer.ContainsKey(gazer) == false)
                {
                    if (debugSort) Debug.LogError("Gazer requesting after sort start. Waiting");
                    continue;
                }
                int gazerIndex = gazerIndexByGazer[gazer];
                var validTargetInfos = gazeTargetInfosByGazerIndex[gazerIndex];
                var newValidTargets = new List<GazeTarget>();
                foreach (var info in validTargetInfos)
                {
                    if (gazeTargetByIndexes.ContainsKey(info.index))
                    {
                        var gazeTarget = gazeTargetByIndexes[info.index];
                        if (gazeTarget != null)
                        {
                            newValidTargets.Add(gazeTarget);
                        }
                        else
                        {
                            if (debugSort) Debug.LogError("Gazer list changed during sort: valid gaze target destroyed");
                        }
                    }
                }
                results[gazer] = newValidTargets;
            }
            sorting = false;
            lastSortEnd = Time.time;
            if (debugSort) Debug.LogError("EndSort => "+(Time.realtimeSinceStartup - startTime));

        }
        #endregion
    }
}