using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Wiggle.UI
{
    public class TabNavigationUI : MonoBehaviour
    {
        [System.Serializable]
        public struct TabMapping
        {
            public Button tabButton;
            public TabButtonVisual tabVisual;
            public GameObject panel;
            public string tabName;
        }

        [SerializeField] private List<TabMapping> tabs;

        private string currentTabName;

        private void Start()
        {
            foreach (var tab in tabs)
            {
                string cachedTabName = tab.tabName;

                if (tab.tabButton != null)
                    tab.tabButton.onClick.AddListener(() => SwitchTab(cachedTabName));

                if (tab.tabVisual != null)
                    tab.tabVisual.SetSelected(false);
            }

            if (tabs.Count > 0)
                SwitchTab(tabs[0].tabName);
        }

        public void SwitchTab(string tabName)
        {
            if (currentTabName == tabName)
                return;

            currentTabName = tabName;

            foreach (var tab in tabs)
            {
                bool isActive = tab.tabName == tabName;

                if (tab.panel != null)
                    tab.panel.SetActive(isActive);

                if (tab.tabVisual != null)
                    tab.tabVisual.SetSelected(isActive);

                if (tab.tabButton != null)
                    tab.tabButton.interactable = !isActive;
            }
        }
    }
}