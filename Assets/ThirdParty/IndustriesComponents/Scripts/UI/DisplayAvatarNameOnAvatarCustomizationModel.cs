using Fusion.XR.Shared.Rig;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayAvatarNameOnAvatarCustomizationModel : MonoBehaviour
{
    [SerializeField] private GameObject avatarNameGO;
    [SerializeField] private KeyboardManager keyboardManager;
    private string username;
    private TextMeshPro avatarNameTMP;


    // Start is called before the first frame update
    void Awake()
    {
        if (avatarNameGO)
        {
            avatarNameTMP = avatarNameGO.GetComponentInChildren<TextMeshPro>(true);

            username = PlayerPrefs.GetString(RigInfo.SETTINGS_USERNAME);

            if (username != null && username != "")
            {
                avatarNameTMP.text = username;
                avatarNameGO.SetActive(true);
            }
            else
            {
                avatarNameGO.SetActive(false);
            }
        }


        if (!keyboardManager)
        {
            keyboardManager = transform.root.GetComponentInChildren<KeyboardManager>(true);
        }

        if (!keyboardManager)
            Debug.LogError("KeyboardManager not found ");
        else
        {
            keyboardManager.onBufferChanged.AddListener(UpdateUsernameDisplay);
        }
    }


    public void UpdateUsernameDisplay()
    {
        string username = keyboardManager.Buffer();

        if (avatarNameTMP && username != "")
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
}
