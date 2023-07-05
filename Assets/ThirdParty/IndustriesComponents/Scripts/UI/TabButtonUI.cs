using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/**
 * 
 * TabButtonUI is in charge to update tab butttons status and main panel when the user select a tab button
 * 
 **/

namespace Fusion.Samples.IndustriesComponents
{
    public class TabButtonUI : MonoBehaviour
    {
        List<TabButtonUI> m_SiblingTabButtons = new List<TabButtonUI>();

        public bool DefaultEnabled;
        public GameObject Tab;
        public Image Image;
        public UnityEvent onTabSelected;

        private void Awake()
        {
            m_SiblingTabButtons = new List<TabButtonUI>(transform.parent.GetComponentsInChildren<TabButtonUI>());
            m_SiblingTabButtons.Remove(this);
        }

        private void Start()
        {
            if (DefaultEnabled)
            {
                OnClick();
            }
        }

        public void OnClick()
        {
            SetSelected(true);
            HideOthers();
            onTabSelected.Invoke();
        }

        void SetSelected(bool value)
        {
            Tab.SetActive(value);
            Image.color = new Color(1f, 1f, 1f, value ? 1f : 0.5f);
        }

        void HideOthers()
        {
            foreach (var button in m_SiblingTabButtons)
            {
                button.HidePanel();
            }
        }

        public void HidePanel()
        {
            SetSelected(false);
        }
    }
}
