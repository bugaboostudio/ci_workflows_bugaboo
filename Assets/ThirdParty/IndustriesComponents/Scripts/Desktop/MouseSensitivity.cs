using UnityEngine;
using UnityEngine.UI;

/**
 * 
 * Manage mouse sensitivity slider
 * 
 **/

namespace Fusion.XR.Shared.Desktop
{
    public class MouseSensitivity : MonoBehaviour
    {
        public Slider mouseSensitivitySlider;
        public MouseCamera mouseCamera;
        public float mouseSensitivity { get; private set; }


        private void Awake()
        {
            if (mouseSensitivitySlider == null) Debug.Log("Slider not set");
        }

        // restore previous settings
        private void OnEnable()
        {
            mouseSensitivitySlider.onValueChanged.RemoveListener(SetMouseSensivity);

            // restore saved volume parameters
            RestoreMouseSensivity();

            // Add listeners on volume sliders
            mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensivity);

        }

        // Apply change
        public void SetMouseSensivity(float value)
        {
            Debug.Log($"Set mouse sensivity to {value}");
            PlayerPrefs.SetFloat("MouseSensivity", value);
            mouseCamera.sensitivity = new Vector2(value * 100, value * 100);
        }

        public void RestoreMouseSensivity()
        {
            float previousMouseSensitivity = PlayerPrefs.GetFloat("MouseSensivity", 0.65f);
            Debug.Log($"Restore previous mouse sensitivity : {previousMouseSensitivity}");
            mouseSensitivitySlider.value = previousMouseSensitivity;
            mouseCamera.sensitivity = new Vector2(previousMouseSensitivity * 100, previousMouseSensitivity * 100);
        }

    }
}
