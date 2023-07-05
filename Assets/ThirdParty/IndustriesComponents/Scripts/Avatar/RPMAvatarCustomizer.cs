using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.Samples.IndustriesComponents
{
    /**
     * 
     * RPMAvatarCustomizer provides methods to display the ReadyPlayerMe avatar selected with the UI or found in the player settings.
     * As start, it set the default ReadyPlayerMe avatar if none has been found in player settings.
     * 
     **/

    public class RPMAvatarCustomizer : MonoBehaviour
    {
        private AvatarRepresentation avatarRepresentation;
        public string avatarRPMURL = "";
        private int currentAvatarID = -1;
        public int defaultAvatarID = 0;
        public List<GameObject> avatarModels = new List<GameObject>();

        const string urlProtocol = "https://d1a370nemizbjq.cloudfront.net/";
        const string avatarModelfileExtension = ".glb";

        // IsValidURL checks if the URL parameter is a simple avatar URL
        public bool IsValidURL(string url)
        {
            return (url.Contains(urlProtocol, System.StringComparison.OrdinalIgnoreCase) && url.Contains(avatarModelfileExtension, System.StringComparison.OrdinalIgnoreCase));
        }

        // Start is called before the first frame update
        void Start()
        {
            avatarRepresentation = GetComponent<AvatarRepresentation>();
            if (avatarRepresentation == null) Debug.LogError("avatarRepresentation not found");

            // display a default RPM avatar if it has not been set by user pref
            if (currentAvatarID == -1)
                SelectAvatarModel(defaultAvatarID);

        }

        public void SelectAvatarModel(int avatarID)
        {
            if (currentAvatarID != -1)
                avatarModels[currentAvatarID].SetActive(false);
            avatarModels[avatarID].SetActive(true);
            currentAvatarID = avatarID;
            avatarRPMURL = urlProtocol + avatarModels[avatarID].gameObject.name + avatarModelfileExtension;
            Debug.Log($"Avatar RPM {avatarRPMURL} selected");
        }

        public void SelectAvatarModelWithURL(string url)
        {
            int i = 0;
            foreach (GameObject avatar in avatarModels)
            {
                if (url.Contains(avatar.name, System.StringComparison.OrdinalIgnoreCase))
                {
                    SelectAvatarModel(i);
                    return;
                }
                i++;
            }
        }

    }

}
