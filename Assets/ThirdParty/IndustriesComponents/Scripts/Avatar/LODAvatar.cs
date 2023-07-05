using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 * At start, the script search the AvatarRepresentation in parent to know which type of avatar should be loaded. It can be a simple avatar model or a Ready Player Me model.
 * 
 * Then, materials for body, hair and clothes colors are configured according to the avatar colors.
 * Also, if the avatar model is a simple avatar, the hair mesh is configured with the hair LOD mesh corresponding to the simple avatar model.
 * 
 **/

namespace Fusion.Samples.IndustriesComponents
{
    public class LODAvatar : MonoBehaviour, IAvatarRepresentationListener
    {
        public MeshRenderer hairRenderer;
        public MeshFilter hairMeshFilter;
        public MeshRenderer bodyRenderer;
        public MeshRenderer clothRenderer;

        public AvatarRepresentation avatarRepresentation;

        [Header("Avatar options")]
        public List<Mesh> hairMeshes = new List<Mesh>();

        #region IAvatarRepresentationListener
        public void AvailableAvatarListed(AvatarRepresentation avatarRepresentation)
        {
            foreach (var avatar in avatarRepresentation.availableAvatars)
            {
                if (avatar.AvatarKind == AvatarKind.ReadyPlayerMe && avatar is RPMAvatarLoader)
                {
                    ((RPMAvatarLoader)avatar).readyPlayerMeAvatarLoaded += OnRPMAvatarLoaded;

                }
                if (avatar.AvatarKind == AvatarKind.SimpleAvatar && avatar is SimpleAvatar)
                {
                    ((SimpleAvatar)avatar).onAvatarLoaded += OnSimpleAvatarLoaded;

                }
            }
        }
        #endregion

        private void Awake()
        {
            if (avatarRepresentation == null)
            {
                avatarRepresentation = GetComponentInParent<AvatarRepresentation>();
            }
            if (hairMeshFilter == null)
            {
                hairMeshFilter = hairRenderer.GetComponent<MeshFilter>();
            }
        }

        // Configure the materials when the avatar is loaded
        private void OnRPMAvatarLoaded(RPMAvatarLoader rpmAvatarLoader)
        {
            bodyRenderer.material.color = rpmAvatarLoader.lastAvatarSkinColor;
            clothRenderer.material.color = rpmAvatarLoader.lastAvatarClothColor;
            hairRenderer.material.color = rpmAvatarLoader.lastAvatarHairColor;

            // do no display the hair renderer if avatar hair is transparent (for bald-headed models)
            hairRenderer.enabled = rpmAvatarLoader.lastAvatarHairColor.a != 0;
        }

        // Configure the materials when the avatar is loaded
        void OnSimpleAvatarLoaded(SimpleAvatar simpleAvatar)
        {
            bodyRenderer.sharedMaterial = simpleAvatar.bodyRenderer.sharedMaterial;
            clothRenderer.sharedMaterial = simpleAvatar.clothRenderer.sharedMaterial;
            hairRenderer.sharedMaterial = simpleAvatar.hairRenderer.sharedMaterial;

            if (simpleAvatar.config.hairMesh < hairMeshes.Count)
            {
                hairMeshFilter.sharedMesh = hairMeshes[simpleAvatar.config.hairMesh];
            }
        }
    }
}
