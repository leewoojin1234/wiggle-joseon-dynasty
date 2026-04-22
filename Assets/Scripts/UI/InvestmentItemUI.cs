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
        public TextMeshProUGUI infoText; // 확률, 수익, 시간 정보 요약
        public TextMeshProUGUI upgradeCostText;
        public TextMeshProUGUI startCostText;

        [Header("Controls")]
        public Button upgradeButton;
        public Button startButton;
        public Slider progressSlider;
        public GameObject progressRoot; // 진행 중일 때만 보여줄 게이지 부모

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
            startButton.onClick.AddListener(() => _system.TryStartInvestment(_index));

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
                infoText.text = $"확률: {_data.GetSuccessRate(level) * 100:F0}% | 수익: {NumberFormatter.Format(_data.GetSuccessIncome(level))} | 시간: {_data.GetDuration(level):F1}s";
                upgradeCostText.text = $"{NumberFormatter.Format(_data.GetUpgradeCost(level))} 냥";
                startCostText.text = $"{NumberFormatter.Format(_data.GetInvestmentCost(level))} 냥";
                startButton.interactable = true;
            }
            else
            {
                infoText.text = "먼저 해금이 필요합니다.";
                upgradeCostText.text = $"{NumberFormatter.Format(_data.GetUpgradeCost(1))} 냥";
                startCostText.text = "미해금";
                startButton.interactable = false;
            }
        }

        void Update()
        {
            if (_system == null) return;

            // 1. 버튼 활성화 상태 실시간 체크
            int level = _system.investmentStatus.GetLevel(_index);
            if (level > 0)
            {
                upgradeButton.interactable = _system.gameStatus.money >= _data.GetUpgradeCost(level);
                startButton.interactable = _system.gameStatus.money >= _data.GetInvestmentCost(level);
            }
            else
            {
                upgradeButton.interactable = _system.gameStatus.money >= _data.GetUpgradeCost(1);
            }

            // 2. 진행 상태 표시 (해당 인덱스의 활성 투자 중 가장 시간이 많이 남은 것 표시)
            var active = _system.GetActiveInvestments().FirstOrDefault(a => a.Index == _index);
            if (active != null)
            {
                progressRoot.SetActive(true);
                float totalTime = _data.GetDuration(active.LevelAtStart);
                progressSlider.value = 1f - (active.RemainingTime / totalTime);
            }
            else
            {
                progressRoot.SetActive(false);
            }
        }
    }
}
