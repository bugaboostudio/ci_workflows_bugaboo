using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion.XR.Shared.Rig;
using UnityEngine.Events;

public class SetUsername : MonoBehaviour
{
    private TMP_InputField usernameInputFieldTMP;
    private string username;
    [SerializeField] private TextMeshProUGUI placeHolder;

    private const string placeHolderVR = "Touch this field to display the keyboard";
    private const string placeHolderDesktop = "Enter your nickname...";
    [SerializeField] private KeyboardManager keyboardManager;


    private void Awake()
    {
        if (!placeHolder)
            placeHolder = GetComponentInChildren<TextMeshProUGUI>();
#if UNITY_ANDROID && !UNITY_EDITOR
        placeHolder.text = placeHolderVR;
#else
        placeHolder.text = placeHolderDesktop;
#endif
    }

    private void OnEnable()
    {
        username = PlayerPrefs.GetString(RigInfo.SETTINGS_USERNAME);

        if (!usernameInputFieldTMP)
            usernameInputFieldTMP = GetComponent<TMP_InputField>();
       
        if (!keyboardManager)
        {
            keyboardManager = transform.root.GetComponentInChildren<KeyboardManager>(true);
        }

        if (username != null)
                usernameInputFieldTMP.text = username;
   
        if (!keyboardManager)
            Debug.LogError("KeyboardManager not found ");
        else
        {
            if (username != null)
                keyboardManager.UpdateKeyboardBuffer(username);
            keyboardManager.onBufferChanged.AddListener(UpdateInputField);
        }
    }

    private void OnDisable()
    {
        keyboardManager.onBufferChanged.RemoveListener(UpdateInputField);
    }

    public void UpdateInputField()
    {
        if (usernameInputFieldTMP)
        {
            if (usernameInputFieldTMP.lineLimit == 1)
            {
                string clean_buffer = keyboardManager.Buffer().Replace("\n", "").Replace("\r", "");
                usernameInputFieldTMP.text = clean_buffer;
            }
            else
                usernameInputFieldTMP.text = keyboardManager.Buffer();
        }
        SaveUsername();
    }

    public void UpdateKeyboardBuffer()
    {
        if (usernameInputFieldTMP && keyboardManager)
            keyboardManager.UpdateKeyboardBuffer(usernameInputFieldTMP.text);
       SaveUsername();
    }

    public void SaveUsername()
    {
        PlayerPrefs.SetString(RigInfo.SETTINGS_USERNAME, usernameInputFieldTMP.text);
    }
}
