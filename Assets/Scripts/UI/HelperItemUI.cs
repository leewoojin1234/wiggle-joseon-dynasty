using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Wiggle.Data;
using Wiggle.Systems;

namespace Wiggle.UI
{
    public class HelperItemUI : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI costText;
        public Button upgradeButton;
        public CanvasGroup canvasGroup; // 잠금 상태 표현용

        private HelperSystem _system;
        private HelperData _data;
        private int _index;

        public void Setup(HelperSystem system, HelperData data, int index)
        {
            _system = system;
            _data = data;
            _index = index;

            if (upgradeButton != null)
            {
                upgradeButton.onClick.RemoveAllListeners(); // 중복 방지
                upgradeButton.onClick.AddListener(OnUpgradeClick);
            }

            UpdateUI();
        }

        public void UpdateUI()
        {
            if (_system == null || _data == null) return;

            int level = _system.helperStatus.GetLevel(_index);
            double cost = _data.GetCost(level);

            nameText.text = _data.helperName;
            levelText.text = $"Lv. {level}";
            // N0 대신 F1 등을 사용하여 소수점까지 표시하거나, 반올림 오해를 없앱니다.
            costText.text = $"{cost:F1} 냥";

            // 초기 상태 설정
            RefreshButtonState();
        }

        void Update()
        {
            // 매 프레임 버튼의 활성화 상태만 가볍게 체크
            RefreshButtonState();
        }

        private void RefreshButtonState()
        {
            if (_system == null || _data == null || upgradeButton == null) return;

            int level = _system.helperStatus.GetLevel(_index);
            double cost = _data.GetCost(level);

            // 1. 잠금 해제 여부 (이전 단계 조력자 고용 여부)
            bool isUnlocked = (_index == 0) || (_system.helperStatus.GetLevel(_index - 1) > 0);

            if (canvasGroup != null)
            {
                // 잠금 상태 시각화는 필요할 때만 갱신하는 게 좋지만, 로직 단순화를 위해 유지
                canvasGroup.alpha = isUnlocked ? 1f : 0.4f;
                canvasGroup.interactable = isUnlocked;
                canvasGroup.blocksRaycasts = isUnlocked;
            }

            // 2. 돈이 충분한지 실시간 체크하여 버튼 활성화
            upgradeButton.interactable = isUnlocked && (_system.gameStatus.money >= cost);
        }
        private void OnUpgradeClick()
        {
            if (_system.TryUpgradeHelper(_index))
            {
                // 리스트 전체를 갱신하여 다음 조력자의 잠금 해제 상태 등을 반영
                // HelperListUI가 이벤트를 구독하고 있으므로, 사실 system 호출만으로 충분할 수 있으나
                // 즉각적인 피드백을 위해 명시적으로 UpdateUI를 호출하거나 이벤트를 발생시킵니다.
                UpdateUI();
            }
        }
    }
}
