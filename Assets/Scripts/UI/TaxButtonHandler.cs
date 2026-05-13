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
        public float incomeMultiplier = 100f;
        [Tooltip("세금 클릭 시 민심 감소량 (음수로 입력)")]
        public float minSimPenalty = -10f;

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
            double currentIncomePerSecond = CalculateCurrentIncome();
            double taxReward = currentIncomePerSecond * incomeMultiplier;

            if (taxAmountText != null)
                taxAmountText.text = $"+{NumberFormatter.Format(taxReward)} 냥";
            
            if (penaltyText != null)
                penaltyText.text = $"민심 {minSimPenalty:F0}";
        }

        private double CalculateCurrentIncome()
        {
            // 1. 민심 보정치
            float minSim = status.minSim;
            float sentimentModifier = (minSim >= 50f) 
                ? Mathf.Lerp(1.0f, 1.5f, (minSim - 50f) / 50f) 
                : Mathf.Lerp(0.2f, 1.0f, minSim / 50f);

            // 2. 기본 + 조력자 수익
            double helperIncome = (HelperSystem.Instance != null) ? HelperSystem.Instance.GetCurrentTotalHelperIncome() : 0;
            double totalBaseIncome = settings.baseIncomePerSecond + helperIncome;

            // 3. 최종 초당 수익
            return totalBaseIncome * sentimentModifier;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (status == null || sentimentSystem == null) return;

            double taxReward = CalculateCurrentIncome() * incomeMultiplier;

            // 1. 코어 로직 (폭군 버튼)
            status.AddMoney(taxReward);
            sentimentSystem.AddMinSim(minSimPenalty);

            Debug.Log($"[폭군] 세금 징수! 돈 +{NumberFormatter.Format(taxReward)}, 민심 {minSimPenalty}");
            
            // 자동 저장 (선택 사항)
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSlotIndex != -1)
            {
                SaveManager.Instance.SaveGame(SaveManager.Instance.CurrentSlotIndex);
            }
        }
    }
}
