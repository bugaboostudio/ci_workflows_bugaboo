using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateDirectoryStructure
{
    [MenuItem("Assets/Create/Directory Structure")]
    public static void CreateDirectories()
    {
        string rootPath = "Assets";
        
        CreateDirectory(rootPath, "Scenes");
        CreateDirectory(rootPath, "Scenes/Login");
        CreateDirectory(rootPath, "Scenes/MainLobby");
        CreateDirectory(rootPath, "Scenes/Auditoriums");
        CreateDirectory(rootPath, "Scenes/MeetingRooms");
        CreateDirectory(rootPath, "Scripts");
        CreateDirectory(rootPath, "Scripts/Player/PlayerController");
        CreateDirectory(rootPath, "Scripts/Player/PlayerUI");
        CreateDirectory(rootPath, "Scripts/NPC");
        CreateDirectory(rootPath, "Scripts/InteractiveObjects");
        CreateDirectory(rootPath, "Scripts/Networking/Photon");
        CreateDirectory(rootPath, "Scripts/Networking/Playfab");
        CreateDirectory(rootPath, "Scripts/Managers");
        CreateDirectory(rootPath, "Scripts/Utils");
        CreateDirectory(rootPath, "Materials");
        CreateDirectory(rootPath, "Textures");
        CreateDirectory(rootPath, "Models");
        CreateDirectory(rootPath, "Models/Buildings");
        CreateDirectory(rootPath, "Models/Furniture");
        CreateDirectory(rootPath, "Models/Avatars");
        CreateDirectory(rootPath, "Models/Miscellaneous");
        CreateDirectory(rootPath, "Animations");
        CreateDirectory(rootPath, "Audio");
        CreateDirectory(rootPath, "Audio/Music");
        CreateDirectory(rootPath, "Audio/SFX");
        CreateDirectory(rootPath, "Audio/Voice");
        CreateDirectory(rootPath, "Prefabs");
        CreateDirectory(rootPath, "Prefabs/NPCs");
        CreateDirectory(rootPath, "Prefabs/Furniture");
        CreateDirectory(rootPath, "Prefabs/InteractiveObjects");
        CreateDirectory(rootPath, "Plugins");
        CreateDirectory(rootPath, "Plugins/Photon");
        CreateDirectory(rootPath, "Plugins/Playfab");
        CreateDirectory(rootPath, "Editor");
        CreateDirectory(rootPath, "Resources");
        CreateDirectory(rootPath, "Fonts");
        CreateDirectory(rootPath, "ThirdParty");
        CreateDirectory(rootPath, "UI");
        CreateDirectory(rootPath, "UI/MainMenu");
        CreateDirectory(rootPath, "UI/HUD");
        CreateDirectory(rootPath, "UI/Dialogues");
        CreateDirectory(rootPath, "UI/Notifications");
        
        AssetDatabase.Refresh();
    }

    private static void CreateDirectory(string rootPath, string name)
    {
        string dir = Path.Combine(rootPath, name);
        if (!AssetDatabase.IsValidFolder(dir))
        {
            AssetDatabase.CreateFolder(Path.GetDirectoryName(dir), Path.GetFileName(dir));
        }
    }
}
