using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleEvent : MonoBehaviour
{
    public UnityEvent OnToggleOn;
    public UnityEvent OnToggleOff;

    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            OnToggleOn.Invoke();
        }
        else
        {
            OnToggleOff.Invoke();
        }
    }
}
