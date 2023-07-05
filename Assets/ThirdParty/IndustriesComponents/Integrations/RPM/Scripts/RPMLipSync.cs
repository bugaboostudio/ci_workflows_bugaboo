using ReadyPlayerMe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ReadyPlayerMe.ExtensionMethods;

namespace Fusion.Samples.IndustriesComponents
{
    /**
     * Simple blenshape animation based on voice volume
     */
    public class RPMLipSync : MonoBehaviour
    {
        public AudioSource audioSource;
        private const string MouthOpenBlendShapeName = "mouthOpen";

        public int amplituteMultiplier = 10;
        public float minVoicePercentToDisplay = 0.04f;

        private SkinnedMeshRenderer headMesh;
        private SkinnedMeshRenderer beardMesh;
        private SkinnedMeshRenderer teethMesh;

        private int mouthOpenBlendShapeIndexOnHeadMesh = -1;
        private int mouthOpenBlendShapeIndexOnBeardMesh = -1;
        private int mouthOpenBlendShapeIndexOnTeethMesh = -1;

        // Start is called before the first frame update
        void Start()
        {
            // Source: VoiceHandler from RPM SDK
            GetMeshAndSetIndex(MeshType.HeadMesh, ref headMesh, ref mouthOpenBlendShapeIndexOnHeadMesh);
            GetMeshAndSetIndex(MeshType.BeardMesh, ref beardMesh, ref mouthOpenBlendShapeIndexOnBeardMesh);
            GetMeshAndSetIndex(MeshType.TeethMesh, ref teethMesh, ref mouthOpenBlendShapeIndexOnTeethMesh);
        }

        // Update is called once per frame
        void Update()
        {
            SetBlendshapeWeights(GetAmplitude());
        }

        private float[] audioSample = new float[1024];
        public float GetAmplitude()
        {
            if (audioSource != null && audioSource.clip != null && audioSource.isPlaying)
            {
                float amplitude = 0f;
                audioSource.clip.GetData(audioSample, audioSource.timeSamples);

                foreach (var sample in audioSample)
                {
                    amplitude += Mathf.Abs(sample);
                }

                var level = Mathf.Clamp01(amplitude / audioSample.Length * amplituteMultiplier);
                if (level < minVoicePercentToDisplay) return 0;
                return level;
            }

            return 0;
        }

        #region Blend Shape Movement
        // Source: VoiceHandler from RPM SDK

        private void GetMeshAndSetIndex(MeshType meshType, ref SkinnedMeshRenderer mesh, ref int index)
        {
            mesh = gameObject.GetMeshRenderer(meshType);

            if (mesh != null)
            {
                index = mesh.sharedMesh.GetBlendShapeIndex(MouthOpenBlendShapeName);
            }
        }

        private void SetBlendshapeWeights(float weight)
        {
            SetBlendShapeWeight(headMesh, mouthOpenBlendShapeIndexOnHeadMesh);
            SetBlendShapeWeight(beardMesh, mouthOpenBlendShapeIndexOnBeardMesh);
            SetBlendShapeWeight(teethMesh, mouthOpenBlendShapeIndexOnTeethMesh);

            void SetBlendShapeWeight(SkinnedMeshRenderer mesh, int index)
            {
                if (index >= 0)
                {
                    mesh.SetBlendShapeWeight(index, weight * 100f);
                }
            }
        }
        #endregion
    }

}
