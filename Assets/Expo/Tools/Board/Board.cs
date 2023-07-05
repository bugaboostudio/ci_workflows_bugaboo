using Fusion.Samples.Expo;
using Fusion.Samples.IndustriesComponents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Fusion.XR.Tools
{
    public class Board : MonoBehaviour
    {
        Camera renderCamera;
        public ExpoManagers managers;

        private void Awake()
        {
            renderCamera = GetComponentInChildren<Camera>();
            if (managers == null) managers = (ExpoManagers) ExpoManagers.FindInstance();
            if (managers == null) Debug.LogError("ExpoManagers not found");
        }

        void Recording(bool state)
        {
            if (state)
            {
                if (!renderCamera.enabled) renderCamera.enabled = true;
            }
            if (!state)
            {
                if (renderCamera.enabled) renderCamera.enabled = false;
            }
        }

        private void Update()
        {
            Recording(state: managers.boardManager.ShouldBoardsRecord);
        }
    }
}
