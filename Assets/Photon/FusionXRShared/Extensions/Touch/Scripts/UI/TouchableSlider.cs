using Fusion.XR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Add support for VR touch interaction to an UI slider
 * Auto adapt the touchable slider collider to the actual slider size
 **/

namespace Fusion.XR.Shared.Interaction
{
    public class TouchableSlider : MonoBehaviour
    {
        public Transform sliderStart;
        public Transform sliderEnd;
        public Slider slider;
        public BoxCollider box;
        public RectTransform rectTransform;
        public float sliderStartOriginalX;
        public float sliderEndOriginalX;

        float defaultWidth = 410;
        float defaultHeight = 40;

        bool adaptSize = true;

        Vector3 originalBoxSize;

        Collider lastToucherCollider = null;

        public AudioSource audioSource;
        private SoundManager soundManager;

        public string Type = "OnTouchButton";

        private void Awake()
        {
            if (slider == null) slider = GetComponentInParent<Slider>();
            box = GetComponentInParent<BoxCollider>();
            rectTransform = slider.GetComponent<RectTransform>();
            originalBoxSize = box.size;
            sliderStartOriginalX = sliderStart.localPosition.x;
            sliderEndOriginalX = sliderEnd.localPosition.x;
        }

        void Start()
        {
            if (soundManager == null) soundManager = SoundManager.FindInstance();
        }

        private void OnEnable()
        {
            if (adaptSize)
                StartCoroutine(AdaptSize());
        }

        // Adapt the size of the 3D button collider according to the UI button
        IEnumerator AdaptSize()
        {
            // We have to wait one frame for rect sizes to be properly set by Unity
            yield return new WaitForEndOfFrame();

            float horizontalFactor = rectTransform.rect.size.x / defaultWidth;
            box.size = new Vector3(originalBoxSize.x * horizontalFactor, originalBoxSize.y * rectTransform.rect.size.y / defaultHeight, box.size.z);
            sliderStart.localPosition = new Vector3(horizontalFactor * sliderStartOriginalX, sliderStart.localPosition.y, sliderStart.localPosition.z);
            sliderEnd.localPosition = new Vector3(horizontalFactor * sliderEndOriginalX, sliderEnd.localPosition.y, sliderEnd.localPosition.z);
        }

        private void OnTriggerStay(Collider other)
        {
            Toucher toucher = other.GetComponentInParent<Toucher>();
            if (lastToucherCollider == other || toucher != null)
            {
                lastToucherCollider = other;
                MoveSliderToPosition(other.transform.position);

                toucher.hardwareHand.SendHapticImpulse();
            }
        }

        public void MoveSliderToPosition(Vector3 position)
        {
            float xStart = transform.InverseTransformPoint(sliderStart.position).x;
            float xEnd = transform.InverseTransformPoint(sliderEnd.position).x;
            float x = transform.InverseTransformPoint(position).x;
            float progress = (x - xStart) / (xEnd - xStart);
            slider.value = progress;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<Toucher>() != null)
            {
                if (soundManager) soundManager.PlayOneShot(Type, audioSource);
            }
        }
    }
}
