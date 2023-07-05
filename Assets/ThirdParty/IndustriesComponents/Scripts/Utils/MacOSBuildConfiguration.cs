#if UNITY_EDITOR
#if UNITY_STANDALONE_OSX

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

class MacOSBuildConfiguration : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    #region IPreprocessBuildWithReport
    public int callbackOrder => -100;

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.LogError("OnPreprocessBuild for MacOS");
        if (BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget) == BuildTargetGroup.Standalone && EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneWindows64)
        {
            Debug.LogError("Building for standalone MacOS: OpenXR not yet supported, disabling it");
            // Source: https://answers.unity.com/questions/1776887/editor-scripting-how-to-edit-xr-plug-in-provider-b.html
            UnityEditor.XR.Management.XRGeneralSettingsPerBuildTarget buildTargetSettings = null;
            EditorBuildSettings.TryGetConfigObject(UnityEngine.XR.Management.XRGeneralSettings.k_SettingsKey, out buildTargetSettings);
            UnityEngine.XR.Management.XRGeneralSettings settings = buildTargetSettings.SettingsForBuildTarget(BuildTargetGroup.Standalone);
            if (settings != null)
            {
                foreach (var s in settings.AssignedSettings.activeLoaders) Debug.LogError("Loader: " + s + " /" + s.GetType().FullName);
                UnityEditor.XR.Management.Metadata.XRPackageMetadataStore.RemoveLoader(settings.Manager, "UnityEngine.XR.OpenXR.OpenXRLoader", BuildTargetGroup.Standalone);
            }
        }
    }
#endregion

#region IPostprocessBuildWithReport
    public void OnPostprocessBuild(BuildReport report)
    {
        Debug.LogError("OnPostprocessBuild for MacOS");
        UnityEditor.XR.Management.XRGeneralSettingsPerBuildTarget buildTargetSettings = null;
        EditorBuildSettings.TryGetConfigObject(UnityEngine.XR.Management.XRGeneralSettings.k_SettingsKey, out buildTargetSettings);
        UnityEngine.XR.Management.XRGeneralSettings settings = buildTargetSettings.SettingsForBuildTarget(BuildTargetGroup.Standalone);
        if (settings != null)
        {
            foreach (var s in settings.AssignedSettings.activeLoaders) Debug.LogError("Loader: " + s + " /" + s.GetType().FullName);
            UnityEditor.XR.Management.Metadata.XRPackageMetadataStore.AssignLoader(settings.Manager, "UnityEngine.XR.OpenXR.OpenXRLoader", BuildTargetGroup.Standalone);
        }
    }
#endregion

}
#endif
#endif
