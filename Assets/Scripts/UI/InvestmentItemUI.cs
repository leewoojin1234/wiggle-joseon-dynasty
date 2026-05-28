using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Wiggle.Data;
using Wiggle.Systems;
using Wiggle.Global;
using System.Linq;

namespace Wiggle.UI
{
    public class InvestmentItemUI : MonoBehaviour
    {
        [Header("Texts")]
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI successRateText;
        public TextMeshProUGUI successIncomeText;
        public TextMeshProUGUI durationText;
        public TextMeshProUGUI sentimentChangeText;
        public TextMeshProUGUI upgradeCostText;
        public TextMeshProUGUI startCostText;

        [Header("Controls")]
        public Button upgradeButton;
        public Button startButton;
        public Slider progressSlider;
        public TextMeshProUGUI remainingTimeText;

        private InvestmentSystem _system;
        private InvestmentData _data;
        private int _index;

        public void Setup(InvestmentSystem system, InvestmentData data, int index)
        {
            _system = system;
            _data = data;
            _index = index;

            upgradeButton.onClick.RemoveAllListeners();
            upgradeButton.onClick.AddListener(() => _system.TryUpgradeInvestment(_index));

            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(OnStartClick);

            UpdateStaticUI();
        }

        // 레벨이나 기본 정보 갱신
        public void UpdateStaticUI()
        {
            if (_system == null || _data == null) return;

            int level = _system.investmentStatus.GetLevel(_index);
            
            nameText.text = _data.investmentName;
            levelText.text = $"Lv. {level}";
            
            if (level > 0)
            {
                UpdateDetailTexts(level);
                upgradeCostText.text = $"강화: {NumberFormatter.Format(_data.GetUpgradeCost(level))} 냥";
                startCostText.text = $"시작: {NumberFormatter.Format(_data.GetInvestmentCost(level))} 냥";
            }
            else
            {
                UpdateLockedDetailTexts();
                upgradeCostText.text = $"해금: {NumberFormatter.Format(_data.GetUpgradeCost(1))} 냥";
                startCostText.text = "미해금";
                startButton.interactable = false;
            }
        }

        private void UpdateDetailTexts(int level)
        {
            string successRate = $"확률: {_data.GetSuccessRate(level) * 100:F0}%";
            string successIncome = $"수익: {NumberFormatter.Format(_data.GetSuccessIncome(level))}";
            string duration = $"시간: {_data.GetDuration(level):F1}s";
            string sentimentChange = $"민심: 성공 {FormatSigned(_data.successMinSim)} / 실패 {FormatSigned(_data.failMinSim)}";

            if (successRateText != null)
                successRateText.text = successRate;

            if (successIncomeText != null)
                successIncomeText.text = successIncome;

            if (durationText != null)
                durationText.text = duration;

            if (sentimentChangeText != null)
                sentimentChangeText.text = sentimentChange;
        }

        private void UpdateLockedDetailTexts()
        {
            if (successRateText != null)
                successRateText.text = "확률: -";

            if (successIncomeText != null)
                successIncomeText.text = "수익: -";

            if (durationText != null)
                durationText.text = "시간: -";

            if (sentimentChangeText != null)
                sentimentChangeText.text = "민심: -";
        }

        private string FormatSigned(float value)
        {
            return value >= 0f ? $"+{value:0.#}" : $"{value:0.#}";
        }

        void Update()
        {
            if (_system == null) return;

            // 1. 진행 상태 표시 (해당 인덱스의 활성 투자 중 가장 시간이 많이 남은 것 표시)
            var active = _system.GetActiveInvestments().FirstOrDefault(a => a.Index == _index);

            // 2. 버튼 활성화 상태 실시간 체크
            int level = _system.investmentStatus.GetLevel(_index);
            if (level > 0)
            {
                upgradeButton.interactable = _system.gameStatus.money >= _data.GetUpgradeCost(level);
                startButton.interactable = active == null && _system.gameStatus.money >= _data.GetInvestmentCost(level);
            }
            else
            {
                upgradeButton.interactable = _system.gameStatus.money >= _data.GetUpgradeCost(1);
            }

            if (active != null)
            {
                float totalTime = _data.GetDuration(active.LevelAtStart);
                if (progressSlider != null)
                    progressSlider.value = 1f - (active.RemainingTime / totalTime);

                if (remainingTimeText != null)
                {
                    int remainingSeconds = Mathf.CeilToInt(active.RemainingTime);
                    int totalSeconds = Mathf.CeilToInt(totalTime);
                    remainingTimeText.text = $"{remainingSeconds}/{totalSeconds}";
                }
            }
            else
            {
                if (remainingTimeText != null)
                    remainingTimeText.text = string.Empty;
            }
        }

        private void OnStartClick()
        {
            if (_system == null) return;

            if (_system.TryStartInvestment(_index) && startButton != null)
                startButton.interactable = false;
        }
    }
}
