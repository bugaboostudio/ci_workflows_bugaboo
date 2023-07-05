using Fusion.XR.Shared.Rig;
using UnityEngine;
using UnityEngine.Events;


namespace Fusion.XR.Shared
{
    /**
     * 
     * Script to display an overlay UI to select desktop or VR mode, and active the associated rig, alongside the connexion component
     * 
     **/

    public class RigSelection : MonoBehaviour
    {
        public UnityEvent OnSelectRig;

        public const string RIGMODE_VR = "VR";
        public const string RIGMODE_DESKTOP = "Desktop";
        public const string RIGMODE_MOBILE = "Mobile";
        public const string SETTING_RIGMODE = "RigMode";

        public GameObject connexionHandler;
        public HardwareRig vrRig;
        public HardwareRig desktopRig;
        public HardwareRig mobileRig;
        Camera rigSelectionCamera;

        public bool forceVROnAndroid = true;

        public bool rigSelected = false;

        public enum Mode
        {
            SelectedByUI,
            SelectedByUserPref,
            ForceVR,
            ForceDesktop,
            ForceMobile
        }
        public Mode mode = Mode.SelectedByUI;

        private void Awake()
        {
            rigSelectionCamera = GetComponentInChildren<Camera>();
            if(connexionHandler) connexionHandler.gameObject.SetActive(false);
            vrRig.gameObject.SetActive(false);
            desktopRig.gameObject.SetActive(false);

#if !UNITY_EDITOR && UNITY_ANDROID
            if (forceVROnAndroid)
            {
                EnableVRRig();
                return;
            }
#endif
            if (mode == Mode.ForceVR)
            {
                EnableVRRig();
                return;
            }

            if (mode == Mode.ForceDesktop)
            {
                EnableDesktopRig();
                return;
            }
            if (mode == Mode.ForceMobile)
            {
                EnableMobileRig();
                return;
            }
            // In release build, we replace SelectedByUI by SelectedByUserPref unless overriden
            DisableDebugSelectedByUI();

            if (mode == Mode.SelectedByUserPref)
            {
                var sessionPrefMode = PlayerPrefs.GetString(SETTING_RIGMODE);
                if (sessionPrefMode != "")
                {
                    if (sessionPrefMode == RIGMODE_VR) EnableVRRig();
                    if (sessionPrefMode == RIGMODE_DESKTOP) EnableDesktopRig();
                    if (sessionPrefMode == RIGMODE_MOBILE) EnableMobileRig();

                }
            }
        }

        protected virtual void DisableDebugSelectedByUI()
        {
#if !UNITY_EDITOR
            if (mode == Mode.SelectedByUI) mode = Mode.SelectedByUserPref;
#endif
        }

        protected virtual void OnGUI()
        {
            GUILayout.BeginArea(new Rect(5, 5, Screen.width - 10, Screen.height - 10));
            {
                GUILayout.BeginVertical(GUI.skin.window);
                {

                    if (GUILayout.Button("VR"))
                    {
                        EnableVRRig();
                    }
                    if (GUILayout.Button("Desktop"))
                    {
                        EnableDesktopRig();
                    }
                    if (GUILayout.Button("Mobile"))
                    {
                        EnableMobileRig();
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndArea();
        }

        void EnableVRRig()
        {
            gameObject.SetActive(false);
            vrRig.gameObject.SetActive(true);
            PlayerPrefs.SetString(SETTING_RIGMODE, RIGMODE_VR);
            PlayerPrefs.Save();
            OnRigSelected();
        }
        void EnableDesktopRig()
        {
            gameObject.SetActive(false);
            desktopRig.gameObject.SetActive(true);
            PlayerPrefs.SetString(SETTING_RIGMODE, RIGMODE_DESKTOP);
            PlayerPrefs.Save();
            OnRigSelected();
        }
        void EnableMobileRig()
        {
            gameObject.SetActive(false);
            mobileRig.gameObject.SetActive(true);
            PlayerPrefs.SetString(SETTING_RIGMODE, RIGMODE_DESKTOP);
            PlayerPrefs.Save();
            OnRigSelected();
        }

        void OnRigSelected()
        {
            if(connexionHandler) connexionHandler.gameObject.SetActive(true);
            if (OnSelectRig != null) OnSelectRig.Invoke();
            if (rigSelectionCamera) rigSelectionCamera.gameObject.SetActive(false);
            rigSelected = true;
        }
    }
}