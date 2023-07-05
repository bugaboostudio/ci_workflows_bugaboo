using Fusion;
using Fusion.XR;
using Fusion.XR.Shared.Rig;
using Newtonsoft.Json;
using ReadyPlayerMe;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

/**
	 * 
	 * RPMAvatarLoader is in charge of ReadyPlayerMe avatar loading when an avatar URL modification has been detected in the XRNetworkedRig by AvatarRepresentation (the ChangeAvatar() function is called).,
	 * First, it tries to load the avatar 
	 *		1/ by instantiating a local prefab is the URL is associated to it,
	 *		2/ by cloning an existing avatar if the avatar URL has already been used to download an avatar,
	 *
	 * If the avatarURL is not found in caches, the ReadyPlayerMe GLB file is finaly downloaded.
	 * 
	 * When the ReadyPlayerMe avatar is loaded then we have to :
	 *		- restructure the avatar to make it a children of the networked rig
	 *		- hide the ReadyPlayerMe hands and change the Oculus color hands
	 *		- configure lyp sync and some features (automatic gazer, eye blink, avatar layers, meshes optimization)
	 * 
	 **/

namespace Fusion.Samples.IndustriesComponents
{
	public class RPMAvatarLoader : MonoBehaviour, IAvatar
	{
		private Vector3 readyPlayerMeHeadOffset = new Vector3(0f, -0.38f, -0.079f);

		protected const string readyPlayerMeHead = "Armature/Hips/Spine/Neck";
		protected const string readyPlayerMeEyeLeft = "Head/LeftEye";
		protected const string readyPlayerMeEyeRight = "Head/RightEye";
		protected const string readyPlayerMeAvatarRendererPath = "Avatar_Renderer_Avatar";
		protected const string readyPlayerMeHands = "Avatar_Renderer_Hands";
		protected const string readyPlayerMeFace = "Avatar_Renderer_Head";
		protected const string readyPlayerMeGlasses = "Avatar_Renderer_Avatar_Transparent";
		protected const string readyPlayerMeRightHandV2 = "Armature/Hips/Spine/RightHand";
		protected const string readyPlayerMeLeftHandV2 = "Armature/Hips/Spine/LeftHand";

		private NetworkRig networkRig;

		public bool loadLocalAvatar = true;

		private SoundManager soundManager;

		[Header("Latest avatar info")]
		public GameObject lastAvatarLoaded;
		public string lastAvatarLoadedName;

		public Color lastAvatarSkinColor;
		public Color lastAvatarHairColor;
		public Color lastAvatarClothColor;

		public delegate void LoadedDelegate(RPMAvatarLoader rpmLoader);
		public LoadedDelegate readyPlayerMeAvatarLoaded;

		AvatarRepresentation avatarRepresentation;

		PerformanceManager performanceManager;
		PerformanceManager.TaskToken? avatarLoadingToken;

		static Dictionary<string, GameObject> sharedAvatarCacheByURL = new Dictionary<string, GameObject>();
		static Dictionary<string, AvatarMetaData> sharedAvatarMetadataCacheByURL = new Dictionary<string, AvatarMetaData>();
		List<string> cacheEntriesAdded = new List<string>();

		bool isAvatarLoading = false;
		bool hasAvatarUrlChangedWhileLoading = false;

		public int lodLevel = 0;

		public List<string> avatarAvailableAsPrefabUrls = new List<string>();
		public List<GameObject> avatarAvailableAsPrefabPrefabs = new List<GameObject>();
		public List<string> avatarAvailableAsPrefabMDs = new List<string>();

		public float avatarRPMScalefactor = 1.2f;

		[Tooltip("The parent under which the spawned avatar will be stored. Automatically set if under a network rig")]
		public Transform avatarParent = null;

		LocalAvatarCulling localAvatarCulling;

		// ReadyPlayMe offers two kind of avatar 
		// a first version which include hand mesh renderers
		// a second version without hand mesh renderers
		public enum RPMAvatarKind
		{
			None,
			V1,// avatar with hand mesh renderers
			V2,
			V3// September 2022 avatars, similar to V2, with skin tone in the metadata, and reorganized texture

		}
		public RPMAvatarKind kind = RPMAvatarKind.None;

		[Header("Optional shader replacement")]
		public Shader faceOverridingShader;

		void OnEnable()
		{
			networkRig = GetComponentInParent<NetworkRig>();
			avatarRepresentation = GetComponentInParent<AvatarRepresentation>();
		}

		#region IAvatar
		public AvatarKind AvatarKind => AvatarKind.ReadyPlayerMe;

		[SerializeField]
		private AvatarStatus _avatarStatus = AvatarStatus.RepresentationLoading;
		public AvatarStatus AvatarStatus
		{
			get { return _avatarStatus; }
			set { _avatarStatus = value; }
		}

		public int TargetLODLevel => lodLevel;

		public AvatarUrlSupport SupportForURL(string url)
		{
			// There is no way to determine if an URL is a proper RPM url
			return AvatarUrlSupport.Maybe;
		}

		public GameObject AvatarGameObject => lastAvatarLoaded;

		public bool ShouldLoadLocalAvatar => loadLocalAvatar;

		public string RandomAvatar() { return null; }

		#endregion

		void Awake()
		{
			if (soundManager == null) soundManager = SoundManager.FindInstance();
		}

		// ChangeAvatar :
		// - checks if the avatarURL parameter is valide
		// - checks if avatar representation has to be updated
		// - display low level avatar LOD while nothing is loaded
		// - cancels eventual previous avatar loading request and asks for a loading request
		// - then, launch the avatar loading
		public async void ChangeAvatar(string avatarURL)
		{
			// check if avatarURL is valid
			if ((avatarURL == null) || (avatarURL == ""))
			{
				if (avatarRepresentation)
				{
					// if the avatarURL is not valid, remove the current avatar representation
					avatarRepresentation.RemoveRepresentation(this, lastAvatarLoaded == null ? null : new List<Renderer>(lastAvatarLoaded.GetComponentsInChildren<Renderer>()));
					// replace the avatar represention by a higher avatar LOD representation
					avatarRepresentation.RepresentationUnavailable(this);
					AvatarStatus = AvatarStatus.RepresentationMissing;
				}
			}
			//check if avatar represenatation has to be updated
			else if (avatarURL != lastAvatarLoadedName)
			{
				// Remove the current avatar
				RemoveCurrentAvatarObject();
				kind = RPMAvatarKind.None;
				// Memorize the avatar URL
				lastAvatarLoadedName = avatarURL;
				// display low level avatar LOD while nothing is loaded
				if (avatarRepresentation) avatarRepresentation.LoadingRepresentation(this);
				AvatarStatus = AvatarStatus.RepresentationLoading;

				// Set the performance manager
				if (!performanceManager && networkRig) performanceManager = networkRig.Runner.GetComponentInChildren<PerformanceManager>();

				if (performanceManager)
				{
					// check if a previous avatar load was requested
					if (avatarLoadingToken != null)
					{
						Debug.LogError("Cancelling previous loading request");
						// Cancel previous avatar loading request
						performanceManager.TaskCompleted(avatarLoadingToken);
					}
					// request a new avatar load
					avatarLoadingToken = await performanceManager.RequestToStartTask(PerformanceManager.TaskKind.NetworkRequest);

					// check if avatar load request has been accepted
					if (avatarLoadingToken == null)
					{
						Debug.LogError("Unable to load avatar: no time slot available");
						return;
					}
				}
				else Debug.LogError("No PerformanceManager found !");
				// launch the avatar loading
				LoadAvatar(lastAvatarLoadedName);
			}
		}


		private void OnDestroy()
		{
			// clean caches on destroy
			RemoveCachedEntries();
		}

		// Remove the URL from caches
		void RemoveCachedEntries()
		{
			foreach (var url in cacheEntriesAdded)
			{
				if (sharedAvatarCacheByURL.ContainsKey(url)) sharedAvatarCacheByURL.Remove(url);
				if (sharedAvatarMetadataCacheByURL.ContainsKey(url)) sharedAvatarMetadataCacheByURL.Remove(url);
			}
		}

		// Remove the current avatar
		public void RemoveCurrentAvatar()
		{
			lastAvatarLoadedName = "";
			RemoveCurrentAvatarObject();
		}

		// RemoveCurrentAvatarObject :
		// - clean the caches
		// - disable the avatar automatic gazer
		// - remove the avatar LOD renderers
		// - destroy the gameObject
		void RemoveCurrentAvatarObject()
		{
			// Check if the avatar GameObject exist
			if (lastAvatarLoaded)
			{
				// remove the avatar from the caches
				RemoveCachedEntries();
				Debug.Log("Remove CurrentRPMAvatar");
				// disable the avatar automatic gazer
				Gazer gazer;
				if (networkRig)
					gazer = networkRig.headset.GetComponentInChildren<Gazer>();
				else
					gazer = GetComponentInChildren<Gazer>();
				gazer.gazingTransforms = new List<Transform>();
				gazer.eyeRendererVisibility = null;
				// remove the avatar LOD renderers
				if (avatarRepresentation) avatarRepresentation.RemoveRepresentation(this, new List<Renderer>(lastAvatarLoaded.GetComponentsInChildren<Renderer>()));
				AvatarStatus = AvatarStatus.NotLoaded;
				// Destroy the gameObject
				Destroy(lastAvatarLoaded);
			}
		}

		// TryLoadCachedAvatar checks if the avatarURL was already used to load avatar
		// if the avatarURL is found in caches, then the avatar is cloned 
		private bool TryLoadCachedAvatar(string avatarURL)
		{
			// check if the avatar URL has already been loaded
			if (sharedAvatarCacheByURL.ContainsKey(avatarURL))
			{
				AvatarMetaData md = null;
				// check if the avatar URL entry exist in the Meta data cache
				if (sharedAvatarMetadataCacheByURL.ContainsKey(avatarURL))
					md = sharedAvatarMetadataCacheByURL[avatarURL];

				// check if the avatar really exist in the cache
				if (sharedAvatarCacheByURL[avatarURL] == null)
				{
					// avatar URL not found, so we remove it in both cache
					sharedAvatarCacheByURL.Remove(avatarURL);
					if (md != null) sharedAvatarMetadataCacheByURL.Remove(avatarURL);
				}
				else
				{
					// the avatar URL exist in the cache 
					Debug.Log("Using cached avatar for url:" + avatarURL);
					// clone the avatar found in the cache 
					var originalAvatar = sharedAvatarCacheByURL[avatarURL];
					GameObject avatar = GameObject.Instantiate(originalAvatar);

					isAvatarLoading = true;
					AvatarLoadedCallback(avatar, md);

					// return true because the avatar has been found in the cache
					return true;
				}
			}
			// return false because the avatar has not been found in the cache
			return false;
		}

		// TryLoadAvatarPrefab search for the avatarURL in the prefab avatar list
		// if found, the avatar prefab is instantiated
		public bool TryLoadAvatarPrefab(string avatarURL)
		{
			int index = avatarAvailableAsPrefabUrls.IndexOf(avatarURL);
			// check if avatarURL has been found in the prefab avatar list
			if (index != -1 && avatarAvailableAsPrefabPrefabs.Count > index)
			{
				// avatar found
				Debug.Log("Using prefab avatar for url:" + avatarURL);
				GameObject avatar = GameObject.Instantiate(avatarAvailableAsPrefabPrefabs[index]);
				string mdString = "{\"bodyType\": \"halfbody\"}";// Default
																 // update the prefab metadata table
				if (avatarAvailableAsPrefabMDs.Count > index) mdString = avatarAvailableAsPrefabMDs[index];
				AvatarMetaData md = JsonConvert.DeserializeObject<AvatarMetaData>(mdString);
				AvatarLoadedCallback(avatar, md);
				return true;
			}
			return false;
		}

		// LoadAvatar first tries to load the avatarURL using prefab avatar or cached avatar
		// Then it prepares and launchs the ReadyPlayerMe loader
		private void LoadAvatar(string avatarURL)
		{
			// tries to load the avatarURL using prefab avatar or cached avatar
			if (TryLoadAvatarPrefab(avatarURL))
				return;
			if (TryLoadCachedAvatar(avatarURL))
				return;

			// Check is an avatar loading is in progress
			if (isAvatarLoading)
			{
				hasAvatarUrlChangedWhileLoading = true;
			}
			else
			{
				hasAvatarUrlChangedWhileLoading = false;
			}
			isAvatarLoading = true;

			// Prepare and launch the ReadyPlayerMe loader
			AvatarLoader avatarLoader = new AvatarLoader();
			avatarURL = avatarURL.Replace("%StreamingAssets%", Application.streamingAssetsPath);
			Debug.Log("Loading avatar url:" + avatarURL);
			avatarLoader.LoadAvatar(avatarURL, AvatarImportedCallback, AvatarLoadedCallback);
		}

		// AvatarImportedCallback is called after GLB file is downloaded and imported 
		private void AvatarImportedCallback(GameObject avatar)
		{
			Debug.Log("Avatar Imported!");
		}

		// HideHands hides avatar hands according to ReadyPlayerMe avatar structure
		void HideHands(GameObject avatar)
		{
			Transform version1Hands = avatar.transform.Find(readyPlayerMeHands);
			if (version1Hands != null)
			{
				kind = RPMAvatarKind.V1;
			}
			else
			{
				kind = RPMAvatarKind.V2;
			}

			// for first kind of ReadyPlayerMe avatar, the hand gameobject must be disable
			if (kind == RPMAvatarKind.V1)
			{
				GameObject readyPlayerMeHandsGO = version1Hands.gameObject;
				readyPlayerMeHandsGO.SetActive(false);
			}
			// for second kind of ReadyPlayerMe avatar, the hands gameobjects localscale must be set to zero
			else
			{
				Transform leftHand = avatar.transform.Find(readyPlayerMeLeftHandV2);
				Transform rightHand = avatar.transform.Find(readyPlayerMeRightHandV2);
				foreach (var hand in new Transform[] { leftHand, rightHand })
				{
					// TODO See with RPM if the hands in V2 format can be hidden more properly (without requiring server configuration)
					hand.localScale = Vector3.zero;
					Transform readyPlayerMeHeadTransform = avatar.transform.Find(readyPlayerMeHead);
					if (readyPlayerMeHeadTransform) hand.position = readyPlayerMeHeadTransform.position;
				}
			}
		}

		// FindFaceMeshRenderer search and return the avatar face mesh renderer according to ReadyPlayerMe avatar structure
		SkinnedMeshRenderer FindFaceMeshRenderer(GameObject avatar)
		{
			SkinnedMeshRenderer faceMeshRenderer = null;
			Transform version1Face = avatar.transform.Find(readyPlayerMeFace);
			if (version1Face)
			{
				GameObject readyPlayerMeFaceGO = version1Face.gameObject;
				faceMeshRenderer = readyPlayerMeFaceGO.GetComponentInChildren<SkinnedMeshRenderer>();
			}
			else
			{
				Transform faceRendererTransform = avatar.transform.Find(readyPlayerMeAvatarRendererPath);
				if (faceRendererTransform)
				{
					GameObject readyPlayerMeFaceGO = faceRendererTransform.gameObject;
					faceMeshRenderer = readyPlayerMeFaceGO.GetComponentInChildren<SkinnedMeshRenderer>();
				}
			}
			return faceMeshRenderer;
		}

		// AvatarLoadedCallback is the call back function called when the ReadyPlayerMe avatar is loaded with components and anim controller 

		private void AvatarLoadedCallback(GameObject avatar, AvatarMetaData metaData)
		{
			Debug.Log($"RPM MD: {metaData.OutfitVersion} {metaData.SkinTone}");
			// remove potential previous avatar 
			RemoveCurrentAvatarObject();
			lastAvatarLoaded = avatar;

			Debug.Log("Avatar Loaded!");

			// Hide the ReadyPlayerMe avatar hands
			HideHands(avatar);

			// V3 handling
			if(kind == RPMAvatarKind.V2 && metaData.SkinTone != null && metaData.SkinTone != "")
            {
				kind = RPMAvatarKind.V3;
            }

			// Restructure avatar
			Transform readyPlayerMeHeadTransform = avatar.transform.Find(readyPlayerMeHead);
			if (!readyPlayerMeHeadTransform)
				Debug.LogError("ReadyPlayerMe Head has not been found !");
			else
			{
				if (avatarParent == null && networkRig) avatarParent = networkRig.headset.networkTransform.InterpolationTarget;
				// move the avatar under the XRNetworkedRig
				avatar.transform.SetParent(avatarParent, false);
				avatar.transform.position = avatarParent.position;
				avatar.transform.rotation = avatarParent.rotation;
				var headsetPosition = avatarParent.InverseTransformPoint(readyPlayerMeHeadTransform.position);
				avatar.transform.localPosition = -headsetPosition + readyPlayerMeHeadOffset;
				avatar.transform.localScale = new Vector3(avatarRPMScalefactor, avatarRPMScalefactor, avatarRPMScalefactor);
			}

			// Get the face et glasses renderers, it will be passed in parameter to build the avatar LOD level
			Transform glasses = avatar.transform.Find(readyPlayerMeGlasses);
			Renderer glassesRenderer = null;
			if (glasses) glassesRenderer = glasses.GetComponentInChildren<SkinnedMeshRenderer>();
			SkinnedMeshRenderer faceMeshRenderer = FindFaceMeshRenderer(avatar);

			if (!faceMeshRenderer)
			{
				Debug.LogError("Unable to configure lipsync");
			}

			// override face with an optionnal shader
			if (faceOverridingShader)
			{
				Texture currentTExture = faceMeshRenderer.material.mainTexture;
				faceMeshRenderer.material.shader = faceOverridingShader;
				faceMeshRenderer.material.mainTexture = currentTExture;
				faceMeshRenderer.material.SetFloat("_Smoothness", 0);
			}
			// optimize avatar skinned mesh renderers
			OptimizeAvatarMeshes(avatar);

			// find the hand color from the avatar mesh renderer and update avatar hands
			ChangeHandSkinColor(faceMeshRenderer, metaData);

			// Add the RendererVisible component if not found on the mesh renderer
			RendererVisible rendererVisible = faceMeshRenderer.gameObject.GetComponent<RendererVisible>();
			if (rendererVisible == null) rendererVisible = faceMeshRenderer.gameObject.AddComponent<RendererVisible>();

			// Activate eyes gazer
			ActivateEyes(avatar, readyPlayerMeHeadTransform, rendererVisible);

			// Initialize the Oculus lip synchronisation

#if UNITY_STANDALONE_OSX && !UNITY_EDITOR || UNITY_EDITOR_OSX
			// Oculus lipsync has some incompatibilities with Photon voice on MacOS: we use another lipsync system
			ConfigureSimpleLipsync(avatar);
#else
			ConfigureOculusLipsync(faceMeshRenderer);
#endif
			// Add the eye animation handler
			ConfigureEyeBlink(avatar);

			// Do not display the avatar for the local player
			avatarRepresentation.ConfigureAvatarLayer(avatar);

			// Configure the avatar LOD system properly
			if (avatarRepresentation) avatarRepresentation.RepresentationAvailable(this, new List<Renderer>() { faceMeshRenderer, glassesRenderer });

			// Add the avatar to the caches if the avatarUrl has not changed while loading
			if (hasAvatarUrlChangedWhileLoading)
			{
				Debug.LogError("Unable to cache avatar as the requested URL changed while loading");
			}
			else if (!sharedAvatarCacheByURL.ContainsKey(lastAvatarLoadedName) || sharedAvatarCacheByURL[lastAvatarLoadedName] == null)
			{
				sharedAvatarCacheByURL[lastAvatarLoadedName] = avatar;
				sharedAvatarMetadataCacheByURL[lastAvatarLoadedName] = metaData;
				cacheEntriesAdded.Add(lastAvatarLoadedName);
			}
			isAvatarLoading = false;

			// Inform the listener that the ReadyPlayerMe avatar had been loaded 
			if (readyPlayerMeAvatarLoaded != null) readyPlayerMeAvatarLoaded(this);

			// Audio feedback to inform player that the avatar is loaded
			soundManager.PlayOneShot("OnAvatarLoaded");

			// Inform the task manager when the avatar loading is completed
			if (performanceManager)
			{
				performanceManager.TaskCompleted(avatarLoadingToken);
				avatarLoadingToken = null;
			}

			AvatarStatus = AvatarStatus.RepresentationAvailable;
		}

		// OptimizeAvatarMeshes optimize avatar skinned mesh renderer for each avatar part
		private void OptimizeAvatarMeshes(GameObject avatar)
		{
			SkinnedMeshRenderer avatarSkinnedMeshRenderer;

			foreach (Transform avatarTransforms in avatar.transform)
			{
				avatarSkinnedMeshRenderer = avatarTransforms.GetComponent<SkinnedMeshRenderer>();
				if (avatarSkinnedMeshRenderer)
				{
					avatarSkinnedMeshRenderer.receiveShadows = false;
					avatarSkinnedMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
					avatarSkinnedMeshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
					avatarSkinnedMeshRenderer.sharedMaterial.SetFloat("_Roughness", 1);
					avatarSkinnedMeshRenderer.sharedMaterial.SetTexture("_BumpMap", null);
				}
			}
		}
				
		// ConfigureEyeBlink adds the ReadyPlayerMe eye animation Handler if it is missing
		private void ConfigureEyeBlink(GameObject avatar)
		{
			if (avatar.GetComponent<EyeAnimationHandler>() == null)
			{
				avatar.AddComponent<EyeAnimationHandler>();
			}
		}

		// ChangeHandSkinColor extract the skin, hair & cloth colors from the skinned mesh renderer and record them in the Avatar Representation
		// Then it update the avatar hand color with the skinColor parameter
		void ChangeHandSkinColor(SkinnedMeshRenderer skinnedMeshRenderer, AvatarMetaData metaData)
		{
			if (skinnedMeshRenderer == null)
				Debug.LogError("Skinned Mesh Renderer not found on avatar");
			else
			{
				if (skinnedMeshRenderer.sharedMaterials.Length == 0)
					Debug.LogError("No material found on Skinned Mesh Renderer");
				else
				{
					Material skinMaterial = skinnedMeshRenderer.sharedMaterials[0];
					Texture2D skinTexture = (Texture2D)skinMaterial.GetTexture("_MainTex");
					if (skinTexture == null) skinTexture = (Texture2D)skinMaterial.mainTexture;
					Color skinColor = Color.clear;
					if (kind == RPMAvatarKind.V1)
					{
						skinColor = skinTexture.GetPixel(0, (int)(skinTexture.height * 0.75f));
					}
					else if (kind == RPMAvatarKind.V2)
					{
						skinColor = skinTexture.GetPixel(skinTexture.width - 1, (int)(skinTexture.height * 0.75f));
					}
					else if (kind == RPMAvatarKind.V3)
					{
                        if (metaData.SkinTone == null || metaData.SkinTone == "" || !ColorUtility.TryParseHtmlString(metaData.SkinTone, out skinColor))
                        {
							Debug.LogError("Unable to parse skintone in RPM V3");
							skinColor = skinTexture.GetPixel(0, (int)(skinTexture.height * 0.75f));
						}
					}

					lastAvatarSkinColor = skinColor;
					lastAvatarHairColor = skinTexture.GetPixel(0, skinTexture.height - 1);
					lastAvatarClothColor = skinTexture.GetPixel((int)(skinTexture.width * 0.75f), (int)(skinTexture.height * 0.1f));

					avatarRepresentation.ChangeHandColor(skinColor);
				}
			}
		}

		//  ActivateEyes configure the gazer to activate the eyes movements based on gaze target detection
		void ActivateEyes(GameObject avatar, Transform headTransform, RendererVisible rendererVisible)
		{
			Gazer gazer = null;
			if (networkRig)
				gazer = networkRig.headset.GetComponentInChildren<Gazer>();
			else
				gazer = GetComponentInChildren<Gazer>();
			gazer.eyeRendererVisibility = rendererVisible;
			gazer.gazingTransformOffsets = new List<Vector3>() { new Vector3(90, 0, 0), new Vector3(90, 0, 0) };

			if (gazer)
			{
				List<Transform> eyes = new List<Transform>();
				var eyeLeft = headTransform.Find(readyPlayerMeEyeLeft);
				var eyeRight = headTransform.Find(readyPlayerMeEyeRight);
				if (eyeLeft)
					eyes.Add(eyeLeft);
				else
					Debug.LogError("eyeLeft not found !");
				if (eyeRight)
					eyes.Add(eyeRight);
				else
					Debug.LogError("eyeRight not found !");

				gazer.gazingTransforms = eyes;
			}
			else
				Debug.LogError("gazer not found !");
		}

		// Basic lipsync based on moving the mouth open blendshape based on volume level (required for MacOS, where )
		void ConfigureSimpleLipsync(GameObject avatar)
        {
			RPMLipSync lipsync = avatar.GetComponent<RPMLipSync>();
			if (lipsync == null)
            {
				lipsync = avatar.AddComponent<RPMLipSync>();
			}
			AudioSource audioSource = GetComponentInChildren<AudioSource>();
			lipsync.audioSource = audioSource;
		}

		// ConfigureOculusLipsync intialized the LipSync context morph target with the facial animation blend shapes
		void ConfigureOculusLipsync(SkinnedMeshRenderer skinnedMeshRenderer)
		{
			if (!networkRig) return;
			OVRLipSyncContext lipSyncContext = networkRig.GetComponentInChildren<OVRLipSyncContext>(true);
			OVRLipSyncContextMorphTarget lipSync = networkRig.GetComponentInChildren<OVRLipSyncContextMorphTarget>(true);
			AudioSource audioSource = GetComponentInChildren<AudioSource>();
            if (lipSyncContext == null)
            {
				lipSyncContext = audioSource.gameObject.AddComponent<OVRLipSyncContext>();
				lipSyncContext.audioSource = audioSource;
				// We change the algorithm, as the original is less resource consuming
				lipSyncContext.provider = OVRLipSync.ContextProviders.Original;
				// We ensure to still hear the avatar voice instead of consuming it
				lipSyncContext.audioLoopback = true;
            }
			if (lipSync == null)
			{
				lipSync = audioSource.gameObject.AddComponent<OVRLipSyncContextMorphTarget>();
			}

			lipSync.enabled = true;
			lipSync.enableVisemeTestKeys = false; //Incompatible with new input system

			// Viseme list: https://docs.readyplayer.me/avatar-specification/avatar-configuration
			List<string> facialAnimationBlendShapes = new List<string>() {
			"viseme_sil",
			"viseme_PP",
			"viseme_FF",
			"viseme_TH",
			"viseme_DD",
			"viseme_kk",
			"viseme_CH",
			"viseme_SS",
			"viseme_nn",
			"viseme_RR",
			"viseme_aa",
			"viseme_E",
			"viseme_I",
			"viseme_O",
			"viseme_U"
		};
			int visemeCount = 0;
			lipSync.skinnedMeshRenderer = skinnedMeshRenderer;

			foreach (var facialAnimationBlendShape in facialAnimationBlendShapes)
			{
				int blendShapeIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(facialAnimationBlendShape);
				if (visemeCount < OVRLipSync.VisemeCount)
				{
					lipSync.visemeToBlendTargets[visemeCount] = blendShapeIndex;
				}
				visemeCount++;
			}
		}
	}
}
