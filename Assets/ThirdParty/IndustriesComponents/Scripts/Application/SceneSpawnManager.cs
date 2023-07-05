using Fusion.Samples.IndustriesComponents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
struct SceneSpawnPoint
{
    public string sceneName;
    public Transform spawnPosition;
    public float randomRadius;
}

[System.Serializable]
public struct SpawnPosition {
    public Vector3 position;
    public Quaternion rotation;

}

/**
 * Change the default behavior of the RandomizeStartPosition components, to handle specific spawn position needs, either for:
 * - reconnection (the user should stay in place)
 * - coming from another scene: the scenes SceneSpawnPoint can define where we should appear when coming from another scene 
 */
public class SceneSpawnManager : MonoBehaviour
{
    public const string SETTINGS_RECONNECTION_POSITION = "ReconnectionPosition";
    public static string PreviousScene { get; private set; }
    [SerializeField]  private List<SceneSpawnPoint> scenes = new List<SceneSpawnPoint>();

    public List<RandomizeStartPosition> startPositionHandlers = new List<RandomizeStartPosition>();


    private void OnDestroy()
    {
        // Store the scene name to allow determining where we came from in the next SceneSpawnmanager Awake()
        PreviousScene = gameObject.scene.name;
    }


    private void Awake()
    {
        // Check if we should restore a position after a reconnection request
        if (RestorePositionOnReload()) return;

        // Check if we should restore a position due to the scene we come from
        if (RestoreSceneTransitionPosition()) return;
    }

    /**
     * Store in PlayerPrefs a position, that will be reused once 
     */
    public void SaveReconnectionPosition(Vector3 position, Quaternion rotation)
    {
        PlayerPrefs.SetString(SETTINGS_RECONNECTION_POSITION, JsonUtility.ToJson(new SpawnPosition { position = position, rotation = rotation }));
        PlayerPrefs.Save();
    }

    public SpawnPosition? ConsumeReconnectionPosition()
    {
        var json = PlayerPrefs.GetString(SETTINGS_RECONNECTION_POSITION, null);
        if (json == null || json == "") return null;
        PlayerPrefs.DeleteKey(SETTINGS_RECONNECTION_POSITION);
        PlayerPrefs.Save();
        return JsonUtility.FromJson<SpawnPosition>(json);
    }

    bool RestorePositionOnReload()
    {
        var spawnPositionOpt = ConsumeReconnectionPosition();
        if (spawnPositionOpt != null)
        {
            // Reloading a scene: we restore the position
            var spawnPosition = spawnPositionOpt.GetValueOrDefault();
            var reconnectionPosition = new GameObject("ReconnectionPosition");
            reconnectionPosition.transform.position = spawnPosition.position;
            reconnectionPosition.transform.rotation = spawnPosition.rotation;
            ChangeStartPosition(reconnectionPosition.transform, 0);
            return true;
        }
        return false;
    }

    bool RestoreSceneTransitionPosition()
    {
        Debug.Log($"PreviousScene :  {SceneSpawnManager.PreviousScene}");

        foreach (SceneSpawnPoint scene in scenes)
        {
            if (PreviousScene == scene.sceneName)
            {
                Debug.Log($"Previous Scene was {scene.sceneName} => Set Spawn Position = {scene.spawnPosition.position}");
                ChangeStartPosition(scene.spawnPosition, scene.randomRadius);
                return true;
            }
        }
        return false;
    }

    void ChangeStartPosition(Transform startTransform, float randomRadius)
    {
        foreach (var startPositionHandler in startPositionHandlers)
        {
            startPositionHandler.startCenterPosition = startTransform;
            startPositionHandler.randomRadius = randomRadius;
            startPositionHandler.FindStartPosition();
        }
    }
}
