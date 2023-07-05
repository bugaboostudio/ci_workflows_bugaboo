using UnityEngine;
using TMPro;

/**
 * 
 * DisplayVersion displays the application version in the UI
 *
 **/
namespace Fusion.Samples.IndustriesComponents
{
    public class DisplayVersion : MonoBehaviour
    {
        public TextMeshProUGUI versionTMP;

        // Start is called before the first frame update
        void Start()
        {
            versionTMP = GetComponent<TextMeshProUGUI>();
            versionTMP.text = "Version " + Application.version.ToString();
        }
    }
}
