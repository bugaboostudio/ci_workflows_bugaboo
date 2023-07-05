using Fusion;
using Fusion.XR;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * Allow to touch an object with a Toucher, and trigger events. Should be associated with a trigger Collider
 * 
 * Provide audio and haptic feedback
 */
public class Touchable : SimulationBehaviour
{
    public Material touchMaterial;
    public bool restoreMaterialAtUntouch = false;
    public bool isToggleButton = false;


    private Material materialAtStart;
    private MeshRenderer meshRenderer;
    public AudioSource audioSource;
    private SoundManager soundManager;

    public float timeBetweenTouchTrigger = 0.3f;
    public float timeBetweenUnTouchTrigger = 0.3f;
    private float lastTouchTime;
    private float timeSinceLastTouch;
    private float lastUnTouchTime;
    private float timeSinceLastUnTouch;
    private bool IsTouching = false;
    public bool buttonStatus = false;

    public string Type = "OnTouchButton";

    [SerializeField] private bool playSoundWhenTouched = true;

    [Header("Events")]
    public UnityEvent onTouch;
    public UnityEvent onUnTouch;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();
        if (meshRenderer) materialAtStart = meshRenderer.material;
    }

    private void OnEnable()
    {
        // We need to clear IsTouching status in case the component was disabled because the TriggerExit has not be called
        if (IsTouching && touchers.Count > 0)
        {
            IsTouching = false;
            touchers.Clear();
        }
    }

    void Start()
    {
        if (playSoundWhenTouched && soundManager == null) soundManager = SoundManager.FindInstance();
    }

    [ContextMenu("OnTouch")]
    public void OnTouch()
    {
        if (isToggleButton)
            buttonStatus = !buttonStatus;
        else
            buttonStatus = true;
        if (onTouch != null) onTouch.Invoke();

        UpdateButton();

        if (playSoundWhenTouched && soundManager) soundManager.PlayOneShot(Type, audioSource);
    }

    void UpdateButton()
    {
        if (!meshRenderer) return;
        if (touchMaterial && buttonStatus)
            meshRenderer.material = touchMaterial;
        else if (materialAtStart && !buttonStatus)
            meshRenderer.material = materialAtStart;
    }

    public void SetButtonStatus(bool status)
    {
        buttonStatus = status;
        UpdateButton();
    }

    public void OnUnTouch()
    {
        if (onUnTouch != null) onUnTouch.Invoke();

        if (!isToggleButton)
        {
            buttonStatus = false;
        }

        UpdateButton();
    }


    #region Toucher interface


    List<Toucher> touchers = new List<Toucher>();
    void RegisterToucher(Toucher toucher)
    {
        if (touchers.Contains(toucher)) return;
        touchers.Add(toucher);
    }
    void UnregisterToucher(Toucher toucher)
    {
        if (!touchers.Contains(toucher)) return;
        touchers.Remove(toucher);
    }

    void CheckTouchers()
    {
        // Check if there was a Touch previously
        if (!IsTouching && touchers.Count > 0)
        {
            timeSinceLastTouch = Time.time - lastTouchTime;
            lastTouchTime = Time.time;
            if (timeSinceLastTouch > timeBetweenTouchTrigger)
            {
                foreach (var toucher in touchers)
                {
                    toucher.hardwareHand.SendHapticImpulse();
                }

                OnTouch();
            }
            IsTouching = true;
        }

        // Check if there is a new touch 
        if (IsTouching && touchers.Count == 0)
        {
            timeSinceLastUnTouch = Time.time - lastUnTouchTime;
            lastUnTouchTime = Time.time;
            if (timeSinceLastUnTouch > timeBetweenUnTouchTrigger)
            {
                OnUnTouch();
            }
            IsTouching = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (Runner.IsForward)
        {
            CheckTouchers();
        }
    }

    public void OnTouchStart(Toucher toucher)
    {
        // Store toucher in contact
        RegisterToucher(toucher);

        // If networked, we will analyse it in FUN, otherwise, right now
        if (!Object)
        {
            CheckTouchers();
        }
    }

    public void OnTouchStay(Toucher toucher)
    {
        // Store toucher in contact
        RegisterToucher(toucher);

        // If networked, we will analyse it in FUN, otherwise, right now
        if (!Object)
        {
            CheckTouchers();
        }
    }

    public void OnTouchEnd(Toucher toucher)
    {
        // Forget toucher in contact
        UnregisterToucher(toucher);

        // If networked, we will analyse it in FUN, otherwise, right now
        if (!Object)
        {
            CheckTouchers();
        }
    }


    #endregion

}
