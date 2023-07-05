using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/**
 * 
 * Add a prefab for some UI elements (Button, Slider; inputfield), so that these UI components can also be touched in VR
 *
 **/

namespace Fusion.XR.Shared.Interaction
{
    public class TouchableCanvas : MonoBehaviour
    {
        public GameObject touchableButtonExtensionPrefab;
        public GameObject touchableSliderExtensionPrefab;
        public GameObject touchableInputFieldExtensionPrefab;
        bool didInstallUITouchButton = false;
        public AudioSource audioSource;

        private void Start()
        {
            if (audioSource == null) Debug.LogError("TouchableCanvas : AudioSource not set ! ");
        }
        private void OnEnable()
        {
            InstallUITouchButton();
        }
        void InstallUITouchButton()
        {
            if (didInstallUITouchButton) return;
            didInstallUITouchButton = true;
            if (touchableButtonExtensionPrefab)
            {
                foreach (var button in GetComponentsInChildren<UnityEngine.UI.Button>(true))
                {
                    if (button.GetComponentInChildren<UITouchButton>() != null)
                    {
                        // Already has a UITouchButton component
                        continue;
                    }
                    var touchable = GameObject.Instantiate(touchableButtonExtensionPrefab, button.transform);
                    touchable.transform.position = button.transform.position;
                    touchable.transform.rotation = button.transform.rotation;
                    touchable.transform.localScale = button.transform.localScale;

                    touchable.GetComponent<Touchable>().audioSource = audioSource;
                }
            }
            if (touchableSliderExtensionPrefab)
            {
                foreach (var slider in GetComponentsInChildren<UnityEngine.UI.Slider>(true))
                {
                    if (slider.GetComponentInChildren<TouchableSlider>() != null)
                    {
                        // Already has a TouchableSlider component
                        continue;
                    }
                    var touchable = GameObject.Instantiate(touchableSliderExtensionPrefab, slider.transform);
                    touchable.transform.position = slider.transform.position;
                    touchable.transform.rotation = slider.transform.rotation;
                    touchable.transform.localScale = slider.transform.localScale;

                    touchable.GetComponent<TouchableSlider>().audioSource = audioSource;
                }
            }

            if (touchableInputFieldExtensionPrefab)
            {
                foreach (var inputField in GetComponentsInChildren<TMP_InputField>(true))
                {
                    if (inputField.GetComponentInChildren<TouchableTMPInputField>() != null)
                    {
                        // Already has a TouchableTMPInputField component
                        continue;
                    }
                    var touchable = GameObject.Instantiate(touchableInputFieldExtensionPrefab, inputField.transform);
                    touchable.transform.position = inputField.transform.position;
                    touchable.transform.rotation = inputField.transform.rotation;
                    touchable.transform.localScale = inputField.transform.localScale;

                    touchable.GetComponent<TouchableTMPInputField>().audioSource = audioSource;
                }
            }


        }
    }
}
