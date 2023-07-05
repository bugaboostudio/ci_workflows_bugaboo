using Fusion.XR;
using Fusion.XR.Shared.Rig;
using UnityEngine;

/**
 * Allow to detect Touchable components in contact
 * 
 * Must be store under an XRHardwareHand to manage haptic feedback properly
 */
public class Toucher : MonoBehaviour
{
    public HardwareHand hardwareHand;

    void Awake()
    {
        hardwareHand = GetComponentInParent<HardwareHand>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Touchable otherGameObjectTouchable = other.GetComponent<Touchable>();
        if (otherGameObjectTouchable)
        {
            otherGameObjectTouchable.OnTouchStart(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Touchable otherGameObjectTouchable = other.GetComponent<Touchable>();
        if (otherGameObjectTouchable)
        {
            otherGameObjectTouchable.OnTouchStay(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {

        Touchable otherGameObjectTouchable = other.GetComponent<Touchable>();
        if (otherGameObjectTouchable)
        {
            otherGameObjectTouchable.OnTouchEnd(this);
        }
    }
}
