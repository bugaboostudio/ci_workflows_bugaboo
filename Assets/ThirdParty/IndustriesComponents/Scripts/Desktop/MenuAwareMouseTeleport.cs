using Fusion.XR.Shared.Desktop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Fusion.Samples.IndustriesComponents.Desktop
{
    public class MenuAwareMouseTeleport : MouseTeleport
    {
        public MenuManager menuManager;
        bool mousePointerDisabledByMenu = false;

        protected override void Awake()
        {
            base.Awake();
            if(menuManager == null)
            {
                Debug.LogError("MenuManager not set on MenuAwareMouseTeleport");
                menuManager = FindObjectOfType<MenuManager>();
            }
        }

        protected override void Update()
        {
            // We don't allow teleport or beam-based mouse click when a menu is opened
            if (menuManager && menuManager.IsMainMenuOpened)
            {
                mousePointerDisabledByMenu = true;
            }
            else if (mousePointerDisabledByMenu)
            {
                // We keep the menu disabled while the user has the left button pressed (they probably clicked on a close button on the ui
                //  and we don't want to teleport them unexpectidly when they release the button
                if (!Mouse.current.leftButton.isPressed && !Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    mousePointerDisabledByMenu = false;
                }
            }
            if (mousePointerDisabledByMenu)
            {
                rayBeamer.isRayEnabled = false;
                return;
            }

            base.Update();
        }
    }

}
