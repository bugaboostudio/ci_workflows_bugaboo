using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EventBox : MonoBehaviour
{
    public TMP_Text dateText;
    public TMP_Text hourText;
    public TMP_Text eventNameText;
    public TMP_Text descriptionText;
    public Button eventButton;

    [TextArea]
    public string date;
    [TextArea]
    public string hour;
    [TextArea]
    public string eventName;
    [TextArea]
    public string description;
    [TextArea]
    public string location;

    private void Start()
    {
        eventButton.onClick.AddListener(LoadLocationScene);
        SetEventData();
    }

    public void SetEventData()
    {
        dateText.text = date;
        hourText.text = hour;
        eventNameText.text = eventName;
        descriptionText.text = description;
    }

    private void LoadLocationScene()
    {
        SceneManager.LoadScene(location);
    }
}
