using Fusion.XR;
using Fusion.XR.Shared.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * 
 * AvatarCustomizer is in charge to display the avatar selection UI.
 * It creates the UI dynamically based on the "Simple Avatar" model.
 * Avatar customization are saved/restored using player preference settings.
 * As start, it display the Simple Avatar or the ReadyPlayerMe panel according to avatar model found in player preference settings.
 * 
 **/

namespace Fusion.Samples.IndustriesComponents
{
    public class AvatarCustomizer : MonoBehaviour
    {
        public HorizontalLayoutGroup hairPalette;
        public HorizontalLayoutGroup clothPalette;
        public HorizontalLayoutGroup skinPalette;
        public SimpleAvatar referenceAvatar;
        public Button prefabChoiceButton;

        public string mainSceneName;

        public SimpleAvatarConfig simpleAvatarConfig;
        public RPMAvatarCustomizer rpmAvatarConfig;
        public AvatarRepresentation avatarRepresentation;

        public HardwareRig rig;
        public Camera desktopCamera;

        public GameObject desktopModeConnectButtonSimpleAvatar;
        public GameObject vrModeConnectButtonSimpleAvatar;
        public GameObject desktopModeConnectButtonRPMAvatar;
        public GameObject vrModeConnectButtonRPMAvatar;

        public TabButtonUI tabButtonSimpleAvatar;
        public TabButtonUI tabButtonRPMAvatar;

        public GameObject simpleAvatarModel;
        public GameObject RPMAvatarModels;

        private bool isRPMAvatarTabSelected = false;

        private float uiCameraOffet = 0.3f;


        [Header("VR settings")]
        public bool defaultVRMode = false;
        public GameObject uiRoot;
        public float delayBeforeDisplayingPanel = 2;
        public float delayBeforeFaderFadeOut = 1;
        public float faderFadeOutDuration = 2;
        public float distanceToUser = 0.55f;

        Fader fader;

        private void Awake()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
        defaultVRMode = true;
#endif
#if UNITY_STANDALONE_OSX
        defaultVRMode = false;
        if (vrModeConnectButtonSimpleAvatar) vrModeConnectButtonSimpleAvatar.SetActive(false);
        if (vrModeConnectButtonRPMAvatar) vrModeConnectButtonRPMAvatar.SetActive(false);
#endif
            fader = rig.GetComponentInChildren<Fader>();

            fader.SetFade(1);


            // Configure the UI according to the display mode : VR or Desktop
            if (defaultVRMode)
            {
                // In VR, hide the "Join Desktop Mode" button, disable the desktop camera and enable the local hardware rig
                desktopModeConnectButtonSimpleAvatar.SetActive(false);
                desktopModeConnectButtonRPMAvatar.SetActive(false);
                desktopCamera.gameObject.SetActive(false);
                rig.gameObject.SetActive(true);
            }
            else
            {
                // In Desktop mode, enable the desktop camera and disable the local hardware rig
                desktopCamera.gameObject.SetActive(true);
                rig.gameObject.SetActive(false);
            }

            PlayerPrefs.SetString("RigMode", "");
            if (uiRoot == null) uiRoot = gameObject;

            if (tabButtonSimpleAvatar)
                tabButtonSimpleAvatar.onTabSelected.AddListener(SimpleAvatarTabSelected);

            if (tabButtonRPMAvatar)
                tabButtonRPMAvatar.onTabSelected.AddListener(RPMAvatarTabSelected);

        }

        private void RPMAvatarTabSelected()
        {
            simpleAvatarModel.SetActive(false);
            RPMAvatarModels.SetActive(true);
            isRPMAvatarTabSelected = true;
        }

        private void SimpleAvatarTabSelected()
        {
            simpleAvatarModel.SetActive(true);
            RPMAvatarModels.SetActive(false);
            isRPMAvatarTabSelected = false;
        }

        private void Start()
        {
            // try to restore previous avatar saved in player preferences
            RestoreAvatarFromUserPref();

            // create the hair UI dynamically based on the "Simple Avatar" model
            if (hairPalette)
            {
                int i = 0;

                // Create a button for each hair model found
                foreach (var hairMat in referenceAvatar.hairMaterials)
                {
                    // create the button
                    var b = GameObject.Instantiate(prefabChoiceButton);

                    // Customize the button color
                    b.colors = new ColorBlock
                    {
                        normalColor = hairMat.color,
                        highlightedColor = hairMat.color,
                        pressedColor = hairMat.color,
                        selectedColor = hairMat.color,
                        colorMultiplier = 1
                    };
                    var index = i;
                    b.transform.SetParent(hairPalette.gameObject.transform, false);

                    // Set the hair model when the button is selected
                    b.onClick.AddListener(() =>
                    {
                        SetHairMaterial(index);
                    });
                    i++;
                }
            }

            // create the cloth UI dynamically based on the "Simple Avatar" model
            if (clothPalette)
            {
                int i = 0;
                // For each cloth
                foreach (var mat in referenceAvatar.clothMaterials)
                {
                    // create the button
                    var b = GameObject.Instantiate(prefabChoiceButton);

                    // Customize the button color
                    b.colors = new ColorBlock
                    {
                        normalColor = mat.color,
                        highlightedColor = mat.color,
                        pressedColor = mat.color,
                        selectedColor = mat.color,
                        colorMultiplier = 1
                    };
                    var index = i;
                    b.transform.SetParent(clothPalette.gameObject.transform, false);

                    // Set the cloth model when the button is selected
                    b.onClick.AddListener(() =>
                    {
                        SetClothMaterial(index);
                    });
                    i++;
                }
            }

            // create the skin UI dynamically based on the "Simple Avatar" model
            if (skinPalette)
            {
                int i = 0;
                // For each skin
                foreach (var mat in referenceAvatar.skinMaterials)
                {
                    // create the button
                    var b = GameObject.Instantiate(prefabChoiceButton);

                    // Customize the button color
                    b.colors = new ColorBlock
                    {
                        normalColor = mat.color,
                        highlightedColor = mat.color,
                        pressedColor = mat.color,
                        selectedColor = mat.color,
                        colorMultiplier = 1
                    };
                    var index = i;
                    b.transform.SetParent(skinPalette.gameObject.transform, false);

                    // Set the skin color model when the button is selected
                    b.onClick.AddListener(() =>
                    {
                        SetSkinMaterial(index);
                    });
                    i++;
                }
            }



            if (defaultVRMode)
            {
                startTime = Time.time;
                if (fader)
                {
                    StartCoroutine(fader.Blink(0, delayBeforeFaderFadeOut, faderFadeOutDuration));
                }
            }
        }

        // RestoreAvatarFromUserPref search the avatar URL in the player preferences.
        // if found, the avatar is restored, else a new avatar config is created
        private void RestoreAvatarFromUserPref()
        {

            string avatarURL = PlayerPrefs.GetString(RigInfo.SETTINGS_AVATARURL);

            if (avatarURL != null && avatarURL != "")
            {
                if (SimpleAvatarConfig.IsValidURL(avatarURL))
                {
                    Debug.Log($"Simple Avatar URL detected : {avatarURL}");
                    // previous avatar found, restore the previous avatar settings
                    SimpleAvatarConfig config = SimpleAvatarConfig.FromURL(avatarURL);
                    SetHairMaterial(config.hairMat);
                    SetHairMesh(config.hairMesh);
                    SetClothMaterial(config.clothMat);
                    SetClothMesh(config.clothMesh);
                    SetSkinMaterial(config.skinMat);
                    // Activate the correct tab
                    tabButtonSimpleAvatar.OnClick();
                }
                else
                {
                    if (rpmAvatarConfig.IsValidURL(avatarURL))
                    {
                        Debug.Log($"RPM avatar URL detected : {avatarURL}");
                        // Activate the correct tab
                        tabButtonRPMAvatar.OnClick();
                        // display the correct avatar
                        rpmAvatarConfig.SelectAvatarModelWithURL(avatarURL);
                    }
                    else
                        Debug.LogError($"Avatar URL is not correct  : {avatarURL}");
                }

            }
            else
            {
                Debug.LogError("Previous NOT avatar found, create a new avatar");
                // previous NOT avatar found, create a new avatar
                SimpleAvatarConfig config = new SimpleAvatarConfig();
                SetHairMaterial(config.hairMat);
                SetHairMesh(config.hairMesh);
                SetClothMaterial(config.clothMat);
                SetClothMesh(config.clothMesh);
                SetSkinMaterial(config.skinMat);
            }

        }

        // Set the avatar hair mesh
        public void SetHairMesh(int val)
        {
            if (val >= referenceAvatar.hairMeshes.Count) return;
            simpleAvatarConfig.hairMesh = val;
            referenceAvatar.SetHair(referenceAvatar.hairMeshes[val], null);
        }

        // Set the avatar cloth mesh
        public void SetClothMesh(int val)
        {
            if (val >= referenceAvatar.clothMeshes.Count) return;
            simpleAvatarConfig.clothMesh = val;
            referenceAvatar.SetCloth(referenceAvatar.clothMeshes[val], null);
        }

        // Set the avatar hair material
        public void SetHairMaterial(int val)
        {
            if (val >= referenceAvatar.hairMaterials.Count) return;
            simpleAvatarConfig.hairMat = val;
            referenceAvatar.SetHair(null, referenceAvatar.hairMaterials[val]);
        }

        // Set the avatar cloth material
        public void SetClothMaterial(int val)
        {
            if (val >= referenceAvatar.clothMaterials.Count) return;
            simpleAvatarConfig.clothMat = val;
            referenceAvatar.SetCloth(null, referenceAvatar.clothMaterials[val]);
        }

        // Set the avatar skin material
        public void SetSkinMaterial(int val)
        {
            if (val >= referenceAvatar.skinMaterials.Count) return;
            simpleAvatarConfig.skinMat = val;
            referenceAvatar.SetSkin(referenceAvatar.skinMaterials[val]);
        }

        // ConnectForceVR is called when the user select the "Join in VR" button
        public void ConnectForceVR()
        {
            // backup user's choice
            PlayerPrefs.SetString("RigMode", "VR");
            // Launch the connection process
            Connect();
        }

        // ConnectForceDesktop is called when the user select the "Join in Desktop mode" button
        public void ConnectForceDesktop()
        {
            // backup user's choice
            PlayerPrefs.SetString("RigMode", "Desktop");
            // Launch the connection process
            Connect();
        }

        // Connect launches the connection process according to the VR or Desktop mode
        public void Connect()
        {
            if (rig && rig.isActiveAndEnabled)
            {
                StartCoroutine(ConnectCoroutine());
            }
            else
            {
                DoConnect();
            }

        }

        // ConnectCoroutine start the fadein and launch the connection process
        IEnumerator ConnectCoroutine()
        {
            Debug.Log("Connect coroutine");
            yield return fader.FadeIn(0.8f);
            DoConnect();
        }

        // DoConnect saves the avatar URL in player settings and load the main scene
        void DoConnect()
        {
            Debug.Log("DoConnect");
            if (isRPMAvatarTabSelected)
            {
                Debug.Log("Save RPM avatar model" + rpmAvatarConfig.avatarRPMURL);
                PlayerPrefs.SetString(RigInfo.SETTINGS_AVATARURL, rpmAvatarConfig.avatarRPMURL);
            }
            else
            {
                Debug.LogError("Save Simple Avatar model" + simpleAvatarConfig.URL);
                PlayerPrefs.SetString(RigInfo.SETTINGS_AVATARURL, simpleAvatarConfig.URL);
            }

            PlayerPrefs.Save();
            SceneManager.LoadScene(mainSceneName);
        }

        float startTime;
        bool firstPlaced = false;
        private void Update()
        {
            // do nothing in desktop mode or when is UI has been moved
            if (!defaultVRMode) return;

            // wait before moving the UI in front of the user
            if ((Time.time - startTime) > delayBeforeDisplayingPanel)
            {
                firstPlaced = true;
                CheckIfUIIsInFrontOfUser();
            }
            else
            {
                PlaceCanvasInFrontOfUser();
            }

        }


        [SerializeField] private float minAngleTomoveUI = 90f;
        [SerializeField] private float minHeightDiffTomoveUI = 0.35f;
        [SerializeField] private float UIMoveSpeed = 2f;

        private bool UIRotationInProgress = false;  // flag when rotation is in progress to avoid stopping as soon as minAngleTomoveUI is reached
        private bool UIMoveDistanceInProgress = false;
        private bool UIMoveHeightInProgress = false;

        private void CheckIfUIIsInFrontOfUser()
        {
            if (defaultVRMode)
            {
                CheckAngleBetweenHeadAndUI();
                CheckHeightDifferenceBetweenHeadAndUI();
                CheckDistanceBetweenHeadAndUI();
            }
        }

        private void CheckDistanceBetweenHeadAndUI()
        {
            var canvasDistance = Mathf.Abs(Vector3.Distance(transform.position, rig.headset.transform.position));
            if ((canvasDistance < distanceToUser*0.6f) || (canvasDistance > 2 * distanceToUser) || UIMoveDistanceInProgress)
            {
                UIMoveDistanceInProgress = true;
                PlaceCanvasInFrontOfUser();
            }
            if (UIMoveDistanceInProgress && ((canvasDistance > distanceToUser) && (canvasDistance < distanceToUser * 1.1)) || ((canvasDistance < distanceToUser) && (canvasDistance > distanceToUser * 0.9)))
            {
                UIMoveDistanceInProgress = false;
            }
        }

        private void CheckHeightDifferenceBetweenHeadAndUI()
        {
            var headPosition = rig.headset.transform.position.y - uiCameraOffet;
            float UIHeightDifference = Mathf.Abs(transform.position.y - headPosition);

            if ((UIHeightDifference > minHeightDiffTomoveUI) || UIMoveHeightInProgress)
            {
                UIMoveHeightInProgress = true;
                PlaceCanvasInFrontOfUser();
            }
            if (UIMoveHeightInProgress && UIHeightDifference < 0.05f)
            {
                UIMoveHeightInProgress = false;
            }
        }

        private void CheckAngleBetweenHeadAndUI()
        {
            var headDirection = rig.headset.transform.forward;
            headDirection.y = 0;
            headDirection = headDirection.normalized;
            var screenDirection = transform.forward;
            screenDirection.y = 0;
            float angle = Mathf.Abs(Vector3.Angle(headDirection, screenDirection));
           
            if ((angle > minAngleTomoveUI) || UIRotationInProgress)
            {
                UIRotationInProgress = true;
                PlaceCanvasInFrontOfUser();
            }

            // Stop the rotation when target angle is almost reached
            if (UIRotationInProgress && angle < 5f)
            {
                UIRotationInProgress = false;
            }
        }

        // If VR Mode is enabled, the UI is moved in front of the user
        void PlaceCanvasInFrontOfUser()
        {
            if (defaultVRMode)
            {
                var forward = rig.headset.transform.forward;
                forward = new Vector3(forward.x, 0, forward.z);

                // compute UI rotation
                Quaternion targetRot = Quaternion.LookRotation(forward);
                targetRot = Quaternion.Euler(0, targetRot.eulerAngles.y, 0);
                if (!firstPlaced)
                    uiRoot.transform.rotation = targetRot;
                else
                    uiRoot.transform.rotation = Quaternion.Lerp(uiRoot.transform.rotation, targetRot, UIMoveSpeed * Time.deltaTime);

                // compute UI position
                var canvasPosition = rig.headset.transform.position + distanceToUser * forward;
                var targetPosition = new Vector3(canvasPosition.x, rig.headset.transform.position.y - uiCameraOffet, canvasPosition.z);
                var newPosition = Vector3.Lerp(uiRoot.transform.position, targetPosition, UIMoveSpeed * Time.deltaTime);
                if (!firstPlaced)
                {
                    uiRoot.transform.position = targetPosition;
                }
                else
                {
                    uiRoot.transform.position = newPosition;
                }

            }
        }
    }
}
