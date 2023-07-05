using Fusion.XR;
using Fusion.XR.Shared.Interaction;
using UnityEngine;
using UnityEngine.Events;
using static Fusion.XR.Shared.Interaction.BeamToucher;

/**
 * Allow to touch an object with a BeamToucher, and trigger events. Should be associated with a trigger Collider
 * 
 * Provide visual, audio and haptic feedback
 */
public class BeamTouchable : MonoBehaviour, IBeamHoverListener
{
    public GameObject onHoverVisualFeedback;
    public AudioSource audioSource;
    private SoundManager soundManager;
    public string Type = "OnTouchButton";
    public UnityEvent onBeamRelease = new UnityEvent();


    void Start()
    {
        if (soundManager == null) soundManager = SoundManager.FindInstance();
    }

    public void OnHoverEnd(BeamToucher beamToucher)
    {
        if (onHoverVisualFeedback) 
            onHoverVisualFeedback.SetActive(false);
    }

    public void OnHoverStart(BeamToucher beamToucher)
    {
        if (onHoverVisualFeedback)
            onHoverVisualFeedback.SetActive(true);

        beamToucher.hardwareHand.SendHapticImpulse();

        if (soundManager)
            soundManager.PlayOneShot(Type, audioSource);
    }

    public void OnHoverRelease(BeamToucher beamToucher)
    {
        if (onBeamRelease != null) onBeamRelease.Invoke();
    }
}
