using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/**
 * 
 * FPSDisplay is in charge to display the number of frame per second
 * Also, the number of spawned bots is displayed in editor mode (update every 2 seconds)
 * 
 **/


namespace Fusion.Samples.IndustriesComponents
{
    public class FPSDisplay : MonoBehaviour
    {
        TextMeshProUGUI m_Text;

        // Start is called before the first frame update
        void Awake()
        {
            m_Text = GetComponent<TextMeshProUGUI>();

            StartCoroutine(UpdateTextRoutine());
        }

        // Update is called once per frame
        IEnumerator UpdateTextRoutine()
        {
            while (true)
            {
                float current = (int)(1f / Time.unscaledDeltaTime);

                m_Text.text = current.ToString() + " FPS";

#if UNITY_EDITOR
                if (Bot.BotCount > 0)
                {
                    m_Text.text += $"\n{Bot.BotCount} Bots";
                }
#endif

                yield return new WaitForSeconds(2f);
            }
        }
    }
}
