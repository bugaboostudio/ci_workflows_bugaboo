using Fusion.XR.Shared;
using Fusion.XR.Shared.Rig;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fusion.Samples.Metaverse
{
    /**
     * Change the Fusion room name to manage group sessions
     * 
     * Build a room name with 4 parts:
     * - appId: base of all room names, common throughout the application
     * - spaceId: id associated to the current position (usually, related to the scene)
     * - groupId: id for the current party in the room (or empty for a common public room)
     * - instanceId: additional id to handle things like load balancing for crowded rooms
     * 
     * Prevent the ConnexionManager to start normally to be sure that the room name has been changed properly
     */
    public class Space : MonoBehaviour
    {
        const string SETTINGS_GROUPID = "SPACES_NAVIGATION_GROUPID";

        [Tooltip("The actual Id of this room. If not set, the scene name is used")]
        public string spaceId = "";
        [Tooltip("If not set, the connexion manager room name will be used")]
        public string appId = "";
        [Tooltip("Define a private group, or a shard. Leave empty for a common public room")]
        public string groupId = "";
        [Tooltip("Can be used for load balancing for crowded rooms, ...")]
        public string instanceId = "";
        [Tooltip("If true, the app build version will be added to app id, to avoid ")]
        public bool addVersionToAppId = true;

        [Header("Connection")]
        public ConnectionManager connexionManager;
        public SceneSpawnManager sceneSpawnManager;
        public RigInfo rigInfo;
        bool connectOnStart = false;
        public string RoomName => string.Join("-", new string[]{appId, spaceId, groupId, instanceId}.Where(s => !string.IsNullOrEmpty(s)));

        private void Awake()
        {
            if (sceneSpawnManager == null) sceneSpawnManager = GetComponentInChildren<SceneSpawnManager>();
            if (sceneSpawnManager == null) sceneSpawnManager = FindObjectOfType<SceneSpawnManager>();
            if (connexionManager == null) connexionManager = GetComponent<ConnectionManager>();

            // SpaceId
            if (string.IsNullOrEmpty(spaceId))
            {
                spaceId = SceneManager.GetActiveScene().name;
            }

            // GrouId
            LoadGroupId();

            // AppId
            if (connexionManager && string.IsNullOrEmpty(appId))
            {
                appId = connexionManager.roomName;
            }
            if (addVersionToAppId)
            {
                appId += Application.version;
            }

            if (connexionManager && connexionManager.connectOnStart)
            {
                connectOnStart = true;
                connexionManager.connectOnStart = false;
                connexionManager.roomName = RoomName;
            }

            if (rigInfo == null && connexionManager) rigInfo = connexionManager.GetComponentInChildren<RigInfo>();
        }

        private async void Start()
        {
            if (connectOnStart)
            {
                await connexionManager.Connect();
            }
        }

        void LoadGroupId()
        {
            groupId = PlayerPrefs.GetString(SETTINGS_GROUPID);
        }


        async void ReloadScene()
        {
            Debug.LogError("Disconnecting...");
            await connexionManager.runner.Shutdown();
            Debug.LogError("Disconnected");
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        IEnumerator Disconnection()
        {
            if (!rigInfo) yield break;
            if (sceneSpawnManager)
            {
                sceneSpawnManager.SaveReconnectionPosition(rigInfo.localHardwareRig.transform.position, rigInfo.localHardwareRig.transform.rotation);
            }
            else
            {
                Debug.LogError("Unable to save reconnection position: no sceneSpawnManager in scene");
            }
            yield return rigInfo.localHardwareRig.headset.fader.FadeIn();
            ReloadScene();
        }

        /**
         * Change the group id (for instance to go from public space to private space)
         * Disconnect the user and reload the scene
         * Save the reconnection position, and it will be restored in scene (if a spawn manager is available)
         */
        public void ChangeGroupId(string gid)
        {
            PlayerPrefs.SetString(SETTINGS_GROUPID, gid);
            PlayerPrefs.Save();
            StartCoroutine(Disconnection());
        }

#if UNITY_EDITOR
        [ContextMenu("Save current groupId")]
        void SaveGroupId(){
            ChangeGroupId(groupId);
        }        
        
        [ContextMenu("Reset groupId")]
        void ResetGroupId(){
            PlayerPrefs.SetString(SETTINGS_GROUPID, "");
            PlayerPrefs.Save();
        }
#endif
    }
}

