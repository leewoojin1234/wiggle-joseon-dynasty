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
            public GameObject panel;
            public string tabName;
        }

        public List<TabMapping> tabs;
        public Color activeColor = Color.white;
        public Color inactiveColor = Color.gray;

        void Start()
        {
            // 모든 탭 버튼에 이벤트 리스너 등록
            foreach (var tab in tabs)
            {
                tab.tabButton.onClick.AddListener(() => SwitchTab(tab.tabName));
            }

            // 기본적으로 첫 번째 탭 활성화
            if (tabs.Count > 0)
            {
                SwitchTab(tabs[0].tabName);
            }
        }

        public void SwitchTab(string tabName)
        {
            foreach (var tab in tabs)
            {
                bool isActive = (tab.tabName == tabName);
        
                // 1. 패널 On/Off
                if (tab.panel != null)
                {
                    tab.panel.SetActive(isActive);
                }

                // 2. 버튼 시각적 피드백 및 인터렉션 제어
                if (tab.tabButton != null)
                {
                    // 선택된 탭이면 interactable을 false로, 아니면 true로 설정
                    tab.tabButton.interactable = !isActive;
                }
            }
        }
    }
}
