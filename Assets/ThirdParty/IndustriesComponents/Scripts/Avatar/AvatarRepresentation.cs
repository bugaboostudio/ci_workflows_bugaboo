using Fusion;
using Fusion.XR;
using Fusion.XR.Shared;
using Fusion.XR.Shared.Rig;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/**
 * 
 *  AvatarRepresentation : 
 *  - listens the XRNetworkedRig onUserAvatarChange event and replace the current avatar by the new one. 
 *	- changes the avatar hands' material & color (for "simple avatar" or ReadyPlayerMe avatar)
 *	- manage the avatar LOD
 * 
 **/

namespace Fusion.Samples.IndustriesComponents
{
	public enum AvatarKind
	{
		None,
		SimpleAvatar,
		ReadyPlayerMe
	}

	public enum AvatarStatus
	{
		NotLoaded,
		RepresentationAvailable,
		RepresentationLoading,
		RepresentationMissing
	}

	public enum AvatarUrlSupport
    {
		Compatible,
		Incompatible,
		Maybe
    }

	public interface IAvatar
    {
		public AvatarKind AvatarKind { get; }
		public AvatarStatus AvatarStatus { get; }
		public int TargetLODLevel { get; }
		public bool ShouldLoadLocalAvatar { get; }
		public AvatarUrlSupport SupportForURL(string url);
		// Launch the avatar loading. When loading, the IAvatar has to call back RepresentationAvailable
		public void ChangeAvatar(string avatarURL);
		public void RemoveCurrentAvatar();
		public GameObject AvatarGameObject { get; }
		// If the avatar system does not support random avatar, should return null
		public string RandomAvatar();
	}

	public interface IAvatarRepresentationListener
    {
		public void AvailableAvatarListed(AvatarRepresentation avatarRepresentation);
	}

	public class AvatarRepresentation : SimulationBehaviour
	{

		public IAvatar currentAvatar;
		public List<IAvatar> availableAvatars = new List<IAvatar>();
		[Tooltip("If true, the LOD won't be used (unless the LOD level 0 avatar is loading)")]
		bool ignoreDistance = false;

		LODGroup lod;
		public NetworkRig networkRig;
		public bool hideLocalHandWhenAvatarWithHandRepresentationIsLoaded = true;

		LocalAvatarCulling localAvatarCulling;

		NetworkHandRepresentationManager _leftNetworkHandRepresentationManager;
		NetworkHandRepresentationManager _rightNetworkHandRepresentationManager;
		HardwareHandRepresentationManager _leftHardwareHandRepresentationManager;
		HardwareHandRepresentationManager _rightHardwareHandRepresentationManager;
		NetworkHandRepresentationManager LeftNetworkHandRepresentationManager
        {
			get
            {
				if (_leftNetworkHandRepresentationManager == null)
                {
					if (!networkRig) return null;
					_leftNetworkHandRepresentationManager = networkRig.leftHand.GetComponentInChildren<NetworkHandRepresentationManager>();
				}
				return _leftNetworkHandRepresentationManager;
			}
		}

		NetworkHandRepresentationManager RightNetworkHandRepresentationManager
		{
			get
			{
				if (_rightNetworkHandRepresentationManager == null)
				{
					if (!networkRig) return null;
					_rightNetworkHandRepresentationManager = networkRig.rightHand.GetComponentInChildren<NetworkHandRepresentationManager>();
				}
				return _rightNetworkHandRepresentationManager;
			}
		}
		HardwareHandRepresentationManager LeftHardwareHandRepresentationManager
        {
			get
            {
				if(_leftHardwareHandRepresentationManager == null)
                {
					if (RigInfo && RigInfo.localHardwareRig)
					{
						_leftHardwareHandRepresentationManager = RigInfo.localHardwareRig.leftHand.GetComponentInChildren<HardwareHandRepresentationManager>();
					}
				}
				return _leftHardwareHandRepresentationManager;
            }
        }
		HardwareHandRepresentationManager RightHardwareHandRepresentationManager
		{
			get
			{
				if (_rightHardwareHandRepresentationManager == null)
				{
					if (RigInfo && RigInfo.localHardwareRig)
					{
						_rightHardwareHandRepresentationManager = RigInfo.localHardwareRig.rightHand.GetComponentInChildren<HardwareHandRepresentationManager>();
					}
				}
				return _rightHardwareHandRepresentationManager;
			}
		}

		RigInfo _rigInfo;

		RigInfo RigInfo
        {
			get
            {
				if(_rigInfo == null)
                {
					if (!networkRig || networkRig.Object == null) return null;
					_rigInfo = RigInfo.FindRigInfo(networkRig.Object.Runner);
				}
				return _rigInfo;
            }
        }

		UserInfo userInfo;
		IAvatarRepresentationListener[] listeners;


		public GameObject avatarNameGO;
		private TextMeshPro avatarNameTMP;


		private void Awake()
		{
			availableAvatars = new List<IAvatar>(GetComponentsInChildren<IAvatar>());

			lod = GetComponentInChildren<LODGroup>();
			networkRig = GetComponentInParent<NetworkRig>();
			listeners = GetComponentsInChildren<IAvatarRepresentationListener>();
			foreach (var listener in listeners) listener.AvailableAvatarListed(this);

			if (avatarNameGO)
				avatarNameTMP = avatarNameGO.GetComponentInChildren<TextMeshPro>();
		}

        void OnEnable()
		{
			if (!networkRig) return;
			userInfo = networkRig.GetComponentInChildren<UserInfo>();

			if (userInfo)
			{
				userInfo.onUserAvatarChange.AddListener(OnUserAvatarChange);
				userInfo.onUserNameChange.AddListener(OnUserNameChange);
			}
		}


		// Disable the LOD (to see the best LOD available all the time)
		public void IgnoreDistance(bool ignore)
        {
			ignoreDistance = ignore;
            if (ignore)
            {
				ForceLOD(0);
            } 
			else
            {
				if(currentAvatar == null || currentAvatar.AvatarStatus == AvatarStatus.RepresentationAvailable)
                {
					ForceLOD(-1);
                }
            }
        }

		// WillLoadAvatar removes the previous loaded avatar if the new one is a different type of avatar
		void WillLoadAvatar(AvatarKind newKind)
		{
            foreach (var avatar in availableAvatars)
            {
				if (newKind != avatar.AvatarKind) avatar.RemoveCurrentAvatar();
			}
		}

		// OnUserAvatarChange replaces the current avatar by the new avatar specified in the XRNetworkedRig
		private void OnUserAvatarChange()
		{
			string avatarURL = userInfo.AvatarURL.ToString();
			ChangeAvatar(avatarURL);
		}

		// OnUserAvatarChange replaces the current username by the new name specified in the XRNetworkedRig/UserInfo 
		void OnUserNameChange()
        {
			ChangeAvatarName(userInfo.UserName);
		}

		void ChangeAvatarName(string username)
		{
			// Display the username
			if (username != "")
			{
				avatarNameGO.SetActive(true);
				if (!avatarNameTMP)
						avatarNameTMP = avatarNameGO.GetComponentInChildren<TextMeshPro>();
				if (avatarNameTMP)
					avatarNameTMP.text = username;
				else
					Debug.LogError("TextMeshPro to display username not found");
			}
			else
				avatarNameGO.SetActive(false);
		}

		
		IAvatar FindSuitableAvatar(string avatarURL, int lodLevel = 0)
        {
			IAvatar compatibleAvatar = null;
			foreach(var avatar in availableAvatars)
            {
				if(avatar.TargetLODLevel != lodLevel) continue;
				var compatibility = avatar.SupportForURL(avatarURL);
				if (compatibility == AvatarUrlSupport.Compatible)
				{
					compatibleAvatar = avatar;
					break;
				}
				if (compatibility == AvatarUrlSupport.Maybe && compatibleAvatar == null)
				{
					compatibleAvatar = avatar;
				}
			}
			return compatibleAvatar;
        }

		public void ChangeAvatar(string avatarURL)
		{
			Debug.Log("ChangeAvatar: "+avatarURL);
			var avatar = FindSuitableAvatar(avatarURL);
			// If loadLocalAvatar is false, do not spawn our local avatar (to be review if a mirror is needed)
			if (!avatar.ShouldLoadLocalAvatar && networkRig.IsLocalNetworkRig)
			{
				return;
			}
			// remove the previous loaded avatar if the new one is a different type of avatar
			WillLoadAvatar(avatar.AvatarKind);
			// load the new avatar. RepresentationAvailable has to be then called (and/or RepresentationUnavailable if needed)
			avatar.ChangeAvatar(avatarURL);
			// memorize the new type of avatar
			currentAvatar = avatar;
		}

		// Called by an avatar system while it is loading an avatar. During this, the avatar is unavailable, so for remote user we want at least the next LOD level to be visible
		public void LoadingRepresentation(IAvatar avatar)
		{
			bool isLocalUser = networkRig.Object.HasInputAuthority;
			if (lod && !isLocalUser)
			{
				// Active low level LOD while nothing is loaded
				RepresentationUnavailable(avatar);
			}
		}

		// If an avatar representation for a LOD level is unavailable (loading, bad url, ...) we ensure that we see the next LOD level instead, by forcing its activation
		public void RepresentationUnavailable(IAvatar avatar)
		{
			int level = avatar.TargetLODLevel;
			if (lod)
			{
				ForceLOD(level + 1);
			}
		}

		// When an avatar has been loaded, we reenable the LOD system properly: we stop forcing it to the next level if we did during the loading, and we add the new avatar renderers the the LODGroup at the desired LOD level
		//  We also hide the offline hand, if needed, if this representation is capable of displaying the avatar hands
		public void RepresentationAvailable(IAvatar avatar, List<Renderer> newRenderers = null, bool includeHandRepresentation = true)
		{
			ConfigureAvatarLayer(avatar.AvatarGameObject);
			int lodLevel = avatar.TargetLODLevel;
			if (lod)
			{
				if (newRenderers != null)
				{
					var currentLods = lod.GetLODs();
					float currentLodObjectSize = lod.size;
					LOD[] lods = new LOD[currentLods.Length];
					foreach (var currentRenderer in currentLods[lodLevel].renderers)
					{
						if (currentRenderer == null)
						{
							Debug.Log("Destroyed renderer");
							continue;
						}
						newRenderers.Add(currentRenderer);
					}
					for (int i = 0; i < currentLods.Length; i++)
					{
						if (i == lodLevel)
						{
							lods[i] = new LOD(currentLods[i].screenRelativeTransitionHeight, newRenderers.ToArray());
						}
						else
						{
							lods[i] = new LOD(currentLods[i].screenRelativeTransitionHeight, currentLods[i].renderers);
						}
					}
					lod.SetLODs(lods);
					ForceLOD(-1);
					lod.size = currentLodObjectSize;
				}
			}

			if (includeHandRepresentation && hideLocalHandWhenAvatarWithHandRepresentationIsLoaded)
			{
				HandleHandWhenAvatarWithHandRepresentationIsLoaded();
			}

			RestoreDistanceHandling();
		}

		void HandleHandWhenAvatarWithHandRepresentationIsLoaded()
        {
			// We handle the hardware hand representation only if online and for the local rig
			if (!RigInfo || RigInfo.localHardwareRig || networkRig.IsLocalNetworkRig == false) return;

			if (LeftHardwareHandRepresentationManager != null && LeftHardwareHandRepresentationManager.localRepresentation != null)
				LeftHardwareHandRepresentationManager.localRepresentation.DisplayMesh(false);
			if (RightHardwareHandRepresentationManager != null && RightHardwareHandRepresentationManager.localRepresentation != null)
				RightHardwareHandRepresentationManager.localRepresentation.DisplayMesh(false);
		}

		void ForceLOD(int level)
        {
			if(lod) lod.ForceLOD(level);
		}

		void RestoreDistanceHandling()
        {
			if (ignoreDistance)
			{
				ForceLOD(0);
			}
			else
			{
				ForceLOD(-1);
			}
		}

		// When an avatar is unloaded, we remove it from the LOD level where it was
		public void RemoveRepresentation(IAvatar avatar, List<Renderer> renderers)
		{
			int level = avatar.TargetLODLevel;
			if (lod)
			{
				var currentLods = lod.GetLODs();
				float currentLodObjectSize = lod.size;
				LOD[] lods = new LOD[currentLods.Length];
				List<Renderer> newRenderers = new List<Renderer>(currentLods[level].renderers);
				foreach (var currentRenderer in currentLods[level].renderers)
				{
					if (currentRenderer == null)
					{
						Debug.LogError("Destroyed renderer");
						continue;
					}
					if (renderers != null && renderers.Contains(currentRenderer))
					{
						newRenderers.Remove(currentRenderer);
					}
				}
				for (int i = 0; i < currentLods.Length; i++)
				{
					if (i == level)
					{
						lods[i] = new LOD(currentLods[i].screenRelativeTransitionHeight, newRenderers.ToArray());
					}
					else
					{
						lods[i] = new LOD(currentLods[i].screenRelativeTransitionHeight, currentLods[i].renderers);
					}
				}
				lod.SetLODs(lods);
				ForceLOD(-1);
				lod.size = currentLodObjectSize;
			}

			RestoreDistanceHandling();
		}

		// change hands skin color with the skinColor parameter
		public void ChangeHandColor(Color skinColor)
		{
			if (!networkRig) return;
			if (LeftNetworkHandRepresentationManager != null && LeftNetworkHandRepresentationManager.handRepresentation != null)
			{
				LeftNetworkHandRepresentationManager.handRepresentation.SetHandColor(skinColor);
			}
			if(RightNetworkHandRepresentationManager != null && RightNetworkHandRepresentationManager.handRepresentation != null)
			{
				RightNetworkHandRepresentationManager.handRepresentation.SetHandColor(skinColor);
			}
			if (networkRig.IsLocalNetworkRig && RigInfo.localHardwareRig)
			{
				if (LeftHardwareHandRepresentationManager != null && LeftHardwareHandRepresentationManager.localRepresentation != null)
				{
					LeftHardwareHandRepresentationManager.localRepresentation.SetHandColor(skinColor);
				}
				if (RightHardwareHandRepresentationManager != null && RightHardwareHandRepresentationManager.localRepresentation != null)
				{
					RightHardwareHandRepresentationManager.localRepresentation.SetHandColor(skinColor);
				}
			}
		}

		// change hands material with the material parameter
		public void ChangeHandMaterial(Material material)
		{
			if (!networkRig) return;
			if (LeftNetworkHandRepresentationManager != null && LeftNetworkHandRepresentationManager.handRepresentation != null)
			{
				LeftNetworkHandRepresentationManager.handRepresentation.SetHandMaterial(material);
			}
			if (RightNetworkHandRepresentationManager != null && RightNetworkHandRepresentationManager.handRepresentation != null)
			{
				RightNetworkHandRepresentationManager.handRepresentation.SetHandMaterial(material);
			}
			if (networkRig.IsLocalNetworkRig && RigInfo.localHardwareRig)
			{
				if (LeftHardwareHandRepresentationManager != null && LeftHardwareHandRepresentationManager.localRepresentation != null)
				{
					LeftHardwareHandRepresentationManager.localRepresentation.SetHandMaterial(material);
				}
				if (RightHardwareHandRepresentationManager != null && RightHardwareHandRepresentationManager.localRepresentation != null)
				{
					RightHardwareHandRepresentationManager.localRepresentation.SetHandMaterial(material);
				}
			}
		}

		// if the avatar type is a simple avatar, RandomAvatar return a random config URL (and set the avatar representation to this one)
		public string RandomAvatar()
		{
			string url = "";
			foreach(var avatar in availableAvatars)
            {
				var avatarUrl = avatar.RandomAvatar();
				if(avatarUrl != "") url = avatarUrl;
            }
			return url;
		}

		// ConfigureAvatarLayer check if the avatar is a local player. If true, avatar renderers are move to a specific layer to be hidden by the local player camera.
		// Else, avatar renderers are set to the Default layer
		public void ConfigureAvatarLayer(GameObject avatar)
		{
			if (networkRig && networkRig.IsLocalNetworkRig)
			{
				if (localAvatarCulling == null)
				{
					RigInfo rigInfo = RigInfo.FindRigInfo(networkRig.Runner);
					localAvatarCulling = rigInfo.localHardwareRig.GetComponent<LocalAvatarCulling>();
				}
				var localAvatarLayer = localAvatarCulling.localAvatarLayer;
				if (localAvatarLayer != "")
				{
					int layer = LayerMask.NameToLayer(localAvatarLayer);
					if (layer == -1)
					{
						Debug.LogError($"Local will be visible and may obstruct you vision. Please add a {localAvatarLayer} layer (it will be automatically removed on the camera culling mask)");
					}
					else
					{
						foreach (var renderer in avatar.GetComponentsInChildren<Renderer>())
						{
							renderer.gameObject.layer = layer;
						}
						avatar.layer = layer;
					}
				}
			}
			else
			{
				int layer = LayerMask.NameToLayer("Default");
				foreach (var renderer in avatar.GetComponentsInChildren<Renderer>())
				{
					renderer.gameObject.layer = layer;
				}
				avatar.layer = layer;
			}
		}
	}
}
