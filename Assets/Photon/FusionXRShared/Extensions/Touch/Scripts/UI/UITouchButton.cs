using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/**
* 
* UITouchButton is used for VR 3D button interaction.
* When the player touches the 3D button box collider, the OnTouch event is forwarded to the UI button
*
**/

namespace Fusion.XR.Shared.Interaction
{
    public class UITouchButton : MonoBehaviour
    {
        public Touchable touchable;
        public Button button;
        public BoxCollider box;
        public RectTransform buttonRectTransform;
        public RectTransform rectTransform;

        public float defaultWidth = 100;
        public float defaultHeight = 100;

        public bool adaptSize = true;

        private void Awake()
        {
            touchable = GetComponent<Touchable>();
            box = GetComponentInParent<BoxCollider>();
            button = GetComponentInParent<Button>();
            touchable.onTouch.AddListener(OnTouch);
            buttonRectTransform = button.GetComponent<RectTransform>();
            rectTransform = GetComponent<RectTransform>();
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

            Vector3 newSize = new Vector3(buttonRectTransform.rect.size.x / button.gameObject.transform.localScale.x, buttonRectTransform.rect.size.y / button.gameObject.transform.localScale.y, box.size.z);
            rectTransform.sizeDelta = new Vector2(newSize.x, newSize.y);
            box.size = newSize;
        }

        // OnTouch event triggered when the player touches the 3D button is forwarded to the UI button 
        private void OnTouch()
        {
            ExecuteEvents.Execute(
                button.gameObject,
                new PointerEventData(EventSystem.current),
                ExecuteEvents.submitHandler);
        }
    }
}
