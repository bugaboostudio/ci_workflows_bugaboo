using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using Fusion.XR;

public class KeyboardManager : MonoBehaviour
{
    [SerializeField] private string buffer;
    [SerializeField] private GameObject LettersKeyboard;
    [SerializeField] private GameObject NumbersKeyboard;
    [SerializeField] private List<TextMeshProUGUI> letters = new List<TextMeshProUGUI>();
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private string audiotype = "OnTouchButton";

    public UnityEvent onBufferChanged;

    private bool capsLock = false;

    [SerializeField] private float timeBetweenTouchTrigger = 0.15f;
    private float lastTouchTime;
    private float timeSinceLastTouch;

    // Start is called before the first frame update
    void Start()
    {
        LettersKeyboard.SetActive(true);
        NumbersKeyboard.SetActive(false);
        if (soundManager == null) soundManager = SoundManager.FindInstance();
        audioSource = GetComponent<AudioSource>();

    }

    public void KeyTouch(TextMeshProUGUI key)
    {
        if (IsTouchTimerExpired())
        {
            buffer += key.text;
            BufferChanged();
        }
    }
   
    public void KeyTouchSpace()
    {
        if (IsTouchTimerExpired())
        {
            buffer = buffer + " ";
            BufferChanged();
        }
    }

    public void KeyTouchReturn()
    {
        if (IsTouchTimerExpired())
        {
            buffer = buffer + Environment.NewLine;
            BufferChanged();
        }
    }

    public void KeyTouchBackSpace()
    {
        if (IsTouchTimerExpired())
        {
            if (buffer.Length > 0)
            {
                buffer = buffer.Substring(0, buffer.Length - 1);
                BufferChanged();
            }
        }
    }

    public void LettersAndNumbersToggle()
    {
        if (IsTouchTimerExpired())
        {
            if (LettersKeyboard.activeSelf)
            {
                LettersKeyboard.SetActive(false);
                NumbersKeyboard.SetActive(true);
            }
            else
            {
                LettersKeyboard.SetActive(true);
                NumbersKeyboard.SetActive(false);
            }
        }
    }

    public void CapsLockToggle()
    {
        if (IsTouchTimerExpired())
        {
            if (capsLock)
            {
                foreach (TextMeshProUGUI letter in letters)
                    letter.text = letter.text.ToLower();

            }
            else
            {
                foreach (TextMeshProUGUI letter in letters)
                    letter.text = letter.text.ToUpper();
            }
            capsLock = !capsLock;
        }
    }

    private void BufferChanged()
    {
        if (onBufferChanged != null) onBufferChanged.Invoke();
        soundManager.PlayOneShot(audiotype, audioSource);
    }

    private bool IsTouchTimerExpired()
    {
        timeSinceLastTouch = Time.time - lastTouchTime;
        lastTouchTime = Time.time;
        if (timeSinceLastTouch > timeBetweenTouchTrigger)
            return true;
        else
            return false;
    }

    public void UpdateKeyboardBuffer(string newbuffer)
    {
        buffer = newbuffer;
        if (onBufferChanged != null) onBufferChanged.Invoke();
    }

    public string Buffer()
    {
        return buffer;
    }

    public void CloseKeyboard()
    {
        if (gameObject.activeSelf)
            gameObject.transform.parent.gameObject.SetActive(false);
    }
}
