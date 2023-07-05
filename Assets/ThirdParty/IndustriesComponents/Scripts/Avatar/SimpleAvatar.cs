using Fusion.XR;
using Fusion.XR.Zone;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 *
 * SimpleAvatarConfig structure encompasses "simple avatar" parameters
 * The URL parameter allows to define a "simple avatar" with the various parameters : hair mesh, hair material, cloth mesh, closh material and skin color
 * 
 **/

namespace Fusion.Samples.IndustriesComponents
{
    [System.Serializable]
    public struct SimpleAvatarConfig
    {
        public int hairMat;
        public int hairMesh;
        public int clothMat;
        public int clothMesh;
        public int skinMat;

        const string urlProtocol = "simpleavatar";
        public string URL => $"{urlProtocol}://?hairMesh={hairMesh}&skinMat={skinMat}&clothMat={clothMat}&hairMat={hairMat}&clothMesh={clothMesh}";

        // IsValidURL checks if the URL parameter is a simple avatar URL
        public static bool IsValidURL(string url)
        {
            return url.Contains(urlProtocol, System.StringComparison.OrdinalIgnoreCase);
        }

        // FromURL parses an URL and returns a SimpleAvatarConfig structure with parameters set according to this URL.
        public static SimpleAvatarConfig FromURL(string url)
        {
            SimpleAvatarConfig config = new SimpleAvatarConfig();
            var urlPart = url.Split('?', 2);
            if (urlPart.Length == 2)
            {
                foreach (var paramInfo in urlPart[1].Split('&'))
                {
                    var paramInfoParts = paramInfo.Split('=', 2);
                    if (paramInfoParts.Length != 2) continue;
                    int val = int.Parse(paramInfoParts[1]);
                    switch (paramInfoParts[0])
                    {
                        case "hairMesh":
                            config.hairMesh = val;
                            break;
                        case "hairMat":
                            config.hairMat = val;
                            break;
                        case "clothMesh":
                            config.clothMesh = val;
                            break;
                        case "clothMat":
                            config.clothMat = val;
                            break;
                        case "skinMat":
                            config.skinMat = val;
                            break;
                    }
                }
            }
            return config;
        }
    }

    public class SimpleAvatar : MonoBehaviour, IAvatar
    {
        /**
        *
        * SimpleAvatar contains methods to : 
        *  - change the avatar thanks to a new "simple avatar" URL
        *  - generate a random "simple avatar" model
        *  - configure a specific avatar parameters (hair, cloth, skin)
        *  - animate avatar's eyes and mouth
        *  
        **/

        public Transform leftEye;
        public Transform rightEye;
        public Renderer hairRenderer;
        public Renderer bodyRenderer;
        public Renderer clothRenderer;
        public Renderer mutedRender;
        public List<Renderer> speakingRenderers = new List<Renderer>();
        public MeshFilter hairFilter;
        public MeshFilter clothFilter;

        AvatarRepresentation avatarRepresentation;
        RendererVisible rendererVisible;

        public int lodLevel = 0;
        public VoiceDetection voiceDetector;
        public bool animateMouth = true;
        public float speakThreshold = 0.001f;
        public float mouthRefreshDelay = 0.1f;

        public GameObject eyeSupport;
        public GameObject eyeLidSupport;

        [Header("Blink")]
        public float minPauseBetweenBlink = 1.5f;
        public float maxPauseBetweenBlink = 3f;
        public float minBlinkDuration = 0.1f;
        public float maxBlinkDuration = 0.2f;

        [Header("Avatar options")]
        public List<Mesh> hairMeshes = new List<Mesh>();
        public List<Material> hairMaterials = new List<Material>();
        public List<Material> skinMaterials = new List<Material>();
        public List<Mesh> clothMeshes = new List<Mesh>();
        public List<Material> clothMaterials = new List<Material>();

        public delegate void LoadedDelegate(SimpleAvatar simpleAvatar);
        public LoadedDelegate onAvatarLoaded;

        public SimpleAvatarConfig config;

        public List<GameObject> avatarParts = new List<GameObject>();

        private bool isMuted = true;
        private float nextMouthUpdate = 0;
        private float nextBlinkStateChangeTime = -1;
        INonUserRig nonUserRig;

        #region IAvatar
        public AvatarKind AvatarKind => AvatarKind.SimpleAvatar;

        public AvatarStatus AvatarStatus => AvatarStatus.RepresentationAvailable;

        public int TargetLODLevel => 0;

        public AvatarUrlSupport SupportForURL(string url)
        {
            if (SimpleAvatarConfig.IsValidURL(url))
                return AvatarUrlSupport.Compatible;
            else
                return AvatarUrlSupport.Incompatible;
        }

        public GameObject AvatarGameObject => gameObject;

        public bool ShouldLoadLocalAvatar => true;
        #endregion


        private void Awake()
        {
            voiceDetector = GetComponentInParent<VoiceDetection>();
            avatarRepresentation = GetComponentInParent<AvatarRepresentation>();
            hairFilter = hairRenderer.GetComponent<MeshFilter>();
            clothFilter = clothRenderer.GetComponent<MeshFilter>();
            foreach (Transform child in transform)
                avatarParts.Add(child.gameObject);
            rendererVisible = GetComponentInChildren<RendererVisible>();
            nonUserRig = GetComponentInParent<INonUserRig>();

        }

        void Start()
        {
            // Disable Voice detection if the avatar is a bot
            if (nonUserRig != null && voiceDetector)
                voiceDetector.enabled = false;
            CloseMouth();
        }


        private void Update()
        {
            // We do not update the avatar if not visisble
            if (rendererVisible != null && (!rendererVisible.isActiveAndEnabled || !rendererVisible.isVisible)) return;

            // Mouth animaton
            if (nextMouthUpdate < Time.time)
            {
                UpdateMouth();
            }

            // Eyes animaton
            if (nextBlinkStateChangeTime < Time.time)
            {
                UpdateEyes();
            }
        }


        // ChangeAvatar update the current avatar with a new one specified in parameter
        // if the avatarURL is not valid, the current avatar is removed
        public void ChangeAvatar(string avatarURL)
        {
            // Check if avatarURL is correct
            if (SimpleAvatarConfig.IsValidURL(avatarURL))
            {
                // The avatarURL is correct
                // Display the avatar parts
                ActivateAvatar();

                // Get the avatar config from the URL and set the corresponding meshes & materials
                config = SimpleAvatarConfig.FromURL(avatarURL);
                if (config.hairMesh <= (hairMeshes.Count - 1)) SetHair(hairMeshes[config.hairMesh], null);
                if (config.hairMat <= (hairMaterials.Count - 1)) SetHair(null, hairMaterials[config.hairMat]);
                if (config.clothMesh <= (clothMeshes.Count - 1)) SetCloth(clothMeshes[config.clothMesh], null);
                if (config.clothMat <= (clothMaterials.Count - 1)) SetCloth(null, clothMaterials[config.clothMat]);
                if (config.skinMat <= (skinMaterials.Count - 1)) SetSkin(skinMaterials[config.skinMat]);

                if (onAvatarLoaded != null) onAvatarLoaded(this);
                if (avatarRepresentation) avatarRepresentation.RepresentationAvailable(this);

            }
            else
            {   // the avatarURL is NOT correct
                // remove the previous avatar
                RemoveCurrentAvatar();
            }

        }



        #region Eyes
        public enum BlinkState
        {
            EyesOpened,
            EyesClosed
        }
        public BlinkState blinkState = BlinkState.EyesOpened;

        // Update the eyes positions according to a random duration
        void UpdateEyes()
        {
            if (blinkState == BlinkState.EyesOpened)
            {
                CloseEyes();
                nextBlinkStateChangeTime = Time.time + Random.Range(minBlinkDuration, maxBlinkDuration);
            }
            else if (blinkState == BlinkState.EyesClosed)
            {
                OpenEyes();
                nextBlinkStateChangeTime = Time.time + Random.Range(minPauseBetweenBlink, maxPauseBetweenBlink);
            }
        }
        // Deactivate eyes and activate eyelids
        void CloseEyes()
        {
            if (!eyeLidSupport || !eyeSupport) return;
            eyeLidSupport.SetActive(true);
            eyeSupport.SetActive(false);
            blinkState = BlinkState.EyesClosed;
        }

        // Activate eyes and deactivate eyelids
        void OpenEyes()
        {
            if (!eyeLidSupport || !eyeSupport) return;
            eyeLidSupport.SetActive(false);
            eyeSupport.SetActive(true);
            blinkState = BlinkState.EyesOpened;
        }

        // ActivateEyes configure the gazer to activate the eyes movements based on gaze target detection
        void ActivateEyes()
        {
            Gazer gazer;
            if (avatarRepresentation.networkRig)
                gazer = avatarRepresentation.networkRig.headset.GetComponentInChildren<Gazer>();
            else
                gazer = avatarRepresentation.GetComponentInChildren<Gazer>();
            RendererVisible rendererVisible = GetComponentInChildren<RendererVisible>();
            gazer.eyeRendererVisibility = rendererVisible;
            gazer.gazingTransformOffsets = new List<Vector3>() { };

            if (gazer && leftEye && rightEye)
            {
                List<Transform> eyes = new List<Transform> { leftEye, rightEye };
                gazer.gazingTransforms = eyes;
            }
        }

        // DisableEyes deactivate the eyes movements based on gaze target detection
        void DisableEyes()
        {
            Gazer gazer = null;
            if (avatarRepresentation.networkRig)
                gazer = avatarRepresentation.networkRig.headset.GetComponentInChildren<Gazer>();
            else
                gazer = avatarRepresentation.GetComponentInChildren<Gazer>();

            if (leftEye && rightEye) gazer.gazingTransforms = new List<Transform>();
            gazer.eyeRendererVisibility = null;
        }
        #endregion

        #region Mouth

        // UpdateMouth animates the avatar mouth according to voice detection
        // The mouth movement is random
        void UpdateMouth()
        {
            nextMouthUpdate = Time.time + mouthRefreshDelay;

            if (!voiceDetector) return;
            if (voiceDetector.voiceVolume < speakThreshold)
            {
                if (!isMuted)
                {
                    CloseMouth();
                }
                isMuted = true;
            }
            else
            {
                if (isMuted)
                {
                    mutedRender.enabled = false;
                }
                isMuted = false;
                ShowMouthStep(Random.Range(0, speakingRenderers.Count));
            }
        }

        // CloseMouth displays the muted mouth renderer and hide others speaking mouth renderers
        void CloseMouth()
        {
            mutedRender.enabled = true;
            ShowMouthStep(-1);
        }

        // ShowMouthStep displays only one speaking mouth renderer
        void ShowMouthStep(int index)
        {
            int i = 0;
            foreach (var renderer in speakingRenderers)
            {
                renderer.enabled = i == index;
                i++;
            }
        }
        #endregion


        [ContextMenu("RandomAvatar")]
        // RandomAvatar generates a random avatar, update the current avatar with the new one and return the corresponding URL
        public string RandomAvatar()
        {
            config.skinMat = Random.Range(0, skinMaterials.Count);
            config.clothMat = Random.Range(0, clothMaterials.Count);
            config.clothMesh = Random.Range(0, clothMeshes.Count);
            config.hairMesh = Random.Range(0, hairMeshes.Count);
            config.hairMat = Random.Range(0, hairMaterials.Count);
            ChangeAvatar(config.URL);
            return config.URL;
        }

        // SetHair configures the avatar hair mesh and material
        public void SetHair(Mesh mesh, Material mat)
        {
            if (mesh != null) hairFilter.sharedMesh = mesh;
            if (mat != null) hairRenderer.sharedMaterial = mat;
        }

        // SetHair configures the cloth mesh and material
        public void SetCloth(Mesh mesh, Material mat)
        {
            if (mesh != null) clothFilter.sharedMesh = mesh;
            if (mat != null) clothRenderer.sharedMaterial = mat;
        }

        // SetHair configures the avatar skin material
        public void SetSkin(Material mat)
        {
            if (mat == null) return;
            bodyRenderer.sharedMaterial = mat;
            if (avatarRepresentation) avatarRepresentation.ChangeHandMaterial(mat);
        }

        // ActivateAvatar enables all avatar parts and activate eyes gazer detection
        public void ActivateAvatar()
        {
            foreach (var part in avatarParts) part.SetActive(true);
            ActivateEyes();
        }

        // RemoveCurrentAvatar disable all avatar parts and deactivate eyes gazer detection 
        public void RemoveCurrentAvatar()
        {
            DisableEyes();
            foreach (var part in avatarParts) part.SetActive(false);
        }
    }
}