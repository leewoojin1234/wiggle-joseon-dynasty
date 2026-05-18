using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Wiggle.Data;
using Wiggle.Global;
using Wiggle.Systems;

namespace Wiggle.UI
{
    public class TaxButtonHandler : MonoBehaviour, IPointerDownHandler
    {
        [Header("Systems & Data")]
        private GameStatus status;
        private GameSettings settings;
        public SentimentSystem sentimentSystem;

        [Header("UI References")]
        public TextMeshProUGUI taxAmountText;
        public TextMeshProUGUI penaltyText;

        [Header("Tax Settings")]
        [Tooltip("현재 초당 수익의 몇 배를 얻을 것인가?")]
        public float incomeMultiplier = 0f;
        [Tooltip("세금 클릭 시 민심 감소량 (음수로 입력)")]
        public float minSimPenalty = 0f;

        private void Awake()
        {
            status ??= DataHub.Status;
            settings ??= DataHub.Settings;
        }

        private void Update()
        {
            UpdateTaxUI();
        }

        private void UpdateTaxUI()
        {
            if (status == null || settings == null) return;

            // 현재 초당 수익 계산 (EconomySystem과 동일 로직)
            float multiplier = GetTaxMultiplier();
            double taxReward = CalculateCurrentIncome() * multiplier;

            if (taxAmountText != null)
                taxAmountText.text = $"+{NumberFormatter.Format(taxReward)} 냥";
            
            if (penaltyText != null)
                penaltyText.text = $"민심 {GetTaxPenalty():F0}";
        }

        private double CalculateCurrentIncome()
        {
            if (EconomySystem.Instance != null)
                return EconomySystem.Instance.CurrentIncomePerSecond;

            return EconomyFormula.GetIncomePerSecond(status, settings, HelperSystem.Instance, InvestmentSystem.Instance);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (status == null) return;

            double taxReward = CalculateCurrentIncome() * GetTaxMultiplier();

            // 1. 코어 로직 (폭군 버튼)
            status.AddMoney(taxReward);
            
            var sSystem = sentimentSystem != null ? sentimentSystem : FindObjectOfType<SentimentSystem>();
            if (sSystem != null)
                sSystem.AddMinSim(GetTaxPenalty());
            else
                status.AddMinSim(GetTaxPenalty());

            Debug.Log($"[폭군] 세금 징수! 돈 +{NumberFormatter.Format(taxReward)}, 민심 {GetTaxPenalty()}");
            
            // 자동 저장 (선택 사항)
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSlotIndex != -1)
            {
                SaveManager.Instance.SaveGame(SaveManager.Instance.CurrentSlotIndex);
            }
        }

        private float GetTaxMultiplier()
        {
            if (incomeMultiplier > 0) return incomeMultiplier;
            return settings != null ? settings.taxIncomeMultiplier : 100f;
        }

        private float GetTaxPenalty()
        {
            if (!Mathf.Approximately(minSimPenalty, 0f)) return minSimPenalty;
            return settings != null ? settings.taxMinSimPenalty : -10f;
        }
    }
}
