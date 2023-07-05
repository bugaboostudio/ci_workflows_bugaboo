using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * 
 * ResolutionDialog provides methods to change the quality & resolution settings and update button status.
 * 
 **/

namespace Fusion.Samples.IndustriesComponents
{
    public class ResolutionDialog : MonoBehaviour
    {
        public Button[] QualityButtons;
        public Color SelectedColor;
        public Color NormalColor;

        void Start()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        Destroy(this.gameObject);
        return;
#endif
            UpdateQualityButtons();
        }

        private void Update()
        {
            if (Screen.width < 640 || Screen.height < 480)
            {
                Screen.SetResolution(640, 480, false);
            }
        }

        // Update the resolution buttons according to the quality settings
        void UpdateQualityButtons()
        {
            for (int i = 0; i < QualityButtons.Length; ++i)
            {
                var colors = QualityButtons[i].colors;
                colors.normalColor = QualitySettings.GetQualityLevel() == i ? SelectedColor : NormalColor;
                colors.selectedColor = QualitySettings.GetQualityLevel() == i ? SelectedColor : NormalColor;
                QualityButtons[i].colors = colors;
            }
        }

        // Change the quality setting and update buttons
        public void SetQualityLevel(int level)
        {
            Debug.Log("Set graphic quality level to : " + level);
            QualitySettings.SetQualityLevel(level, true);
            UpdateQualityButtons();
        }

        // Change resolution mode
        public void SetResolution(float percentage)
        {
            Debug.Log("Set resolution to : " + percentage * 100 + "%");
            int width = (int)(Display.main.systemWidth * percentage);
            int height = (int)(Display.main.systemHeight * percentage);
            FullScreenMode fullScreenMode = FullScreenMode.Windowed;

            if (percentage == 1f)
            {
                fullScreenMode = FullScreenMode.ExclusiveFullScreen;
            }

            Screen.SetResolution(width, height, fullScreenMode);
        }
    }
}
