using UnityEditor;
using UnityEngine;
using UnityEngine.XR.OpenXR.Features;

namespace Fusion.Samples.IndustriesComponents
{
#if UNITY_EDITOR
[UnityEditor.XR.OpenXR.Features.OpenXRFeature(UiName = "Shutdown Feature",
    BuildTargetGroups = new[] { BuildTargetGroup.WSA, BuildTargetGroup.Standalone, BuildTargetGroup.Android },
    Company = "Unknown",
    Desc = "Automatically shuts down the app when exit is requested.",
    Version = "0.0.1",
    FeatureId = featureId)]
#endif
    public class ShutdownFeature : OpenXRFeature
    {
        public const string featureId = "com.unknown.features.shutdown";

        protected override void OnSessionEnd(ulong xrSession)
        {
            base.OnSessionEnd(xrSession);
            Application.Quit();
        }
    }
}

