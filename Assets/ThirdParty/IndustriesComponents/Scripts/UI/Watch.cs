using Fusion.XR;
using Fusion.XR.Shared.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 * Display the current time on the watch, and open the main menu upon watchface click
 * 
 **/

namespace Fusion.Samples.IndustriesComponents
{
    public class Watch : MonoBehaviour
    {
        public TMPro.TextMeshPro timeText;
        public MenuManager menu;
        public HardwareRig rig;
        public float menuVerticalOffset = -0.5f;
        public float menuDistance = 1;
        Vector3 lastPlayAreaPosition;

        private void Awake()
        {
            rig = GetComponentInParent<HardwareRig>();
        }
        private void Update()
        {
            var now = System.DateTime.Now;
            timeText.text = now.ToString("HH:mm:ss");

            if (!menu) return;
            if (menu.IsMainMenuOpened)
            {
                if ((rig.transform.position - lastPlayAreaPosition).sqrMagnitude > 0.01f)
                {
                    // Did Teleport the playarea: we close the menu
                    WatchFaceClick();
                }
            }
        }

        [ContextMenu("Watch face click")]
        public void WatchFaceClick()
        {
            Debug.Log("User opens the menu with the watch");

            if (!menu) return;
            if (menu.IsMainMenuOpened)
            {
                menu.CloseMainMenu();
            }
            else
            {
                menu.OpenMainMenu();
                // Convert the overlay menu to a worldspace menu, attached to the user playarea
                menu.mainMenu.GetComponentInParent<Canvas>().renderMode = RenderMode.WorldSpace;
                var forward = rig.headset.transform.forward;
                forward.y = 0;
                menu.mainMenu.transform.position = rig.headset.transform.position + forward * menuDistance + Vector3.down * menuVerticalOffset;
                menu.mainMenu.transform.LookAt(rig.headset.transform);
                menu.mainMenu.transform.position += Vector3.down * menuVerticalOffset;
                menu.mainMenu.transform.Rotate(0, 180, 0);

                menu.mainMenu.transform.localScale = Vector3.one * 0.00083f;
            }
            lastPlayAreaPosition = rig.transform.position;
        }
    }
}
