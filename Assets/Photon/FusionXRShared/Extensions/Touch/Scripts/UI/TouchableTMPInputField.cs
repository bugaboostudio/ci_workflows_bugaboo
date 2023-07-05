using Fusion.XR;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/**
* 
* TouchableTMPInputField is used for VR 3D button interaction.
* When the player touches the 3D box collider, the keyboard visibility is toggled.
*
**/

public class TouchableTMPInputField : MonoBehaviour
{
    public Touchable touchable;
    public BoxCollider box;
    public RectTransform inputFieldRectTransform;
    public RectTransform rectTransform;
    public TMP_InputField inputfield;
    public float defaultWidth = 100;
    public float defaultHeight = 100;
    public bool adaptSize = true;

    public AudioSource audioSource;
    private SoundManager soundManager;

    [SerializeField] private GameObject keyboard;

    private void Awake()
    {
        inputfield = GetComponentInParent<TMP_InputField>();
        box = GetComponentInParent<BoxCollider>();
        inputFieldRectTransform = inputfield.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
        touchable = GetComponent<Touchable>();
        touchable.onTouch.AddListener(OnTouch);

        if (!keyboard)
            keyboard = transform.root.GetComponentInChildren<KeyboardManager>(true).transform.parent.gameObject;
    }

    private void OnEnable()
    {
        if (adaptSize)
            StartCoroutine(AdaptSize());
    }

    // Adapt the size of the 3D button collider according to the UI
    IEnumerator AdaptSize()
    {
        // We have to wait one frame for rect sizes to be properly set by Unity
        yield return new WaitForEndOfFrame();

        Vector3 newSize = new Vector3(inputFieldRectTransform.rect.size.x / inputfield.gameObject.transform.localScale.x, inputFieldRectTransform.rect.size.y / inputfield.gameObject.transform.localScale.y, box.size.z);
        rectTransform.sizeDelta = new Vector2(newSize.x, newSize.y);
        box.size = newSize;
    }

    // OnTouch event triggered when the player touches the 3D button is forwarded to the UI button 
    private void OnTouch()
    {
        ToggleKeyboard();
    }

    private void ToggleKeyboard()
    {
        if (keyboard.activeSelf)
            keyboard.SetActive(false);
        else
            keyboard.SetActive(true);
    }
}
