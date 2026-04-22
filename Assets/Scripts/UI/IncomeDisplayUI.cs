using UnityEngine;
using TMPro;
using Wiggle.Data;
using Wiggle.Global;

namespace Wiggle.UI
{
    public class IncomeDisplayUI : MonoBehaviour
    {
        [Header("Data Reference")]
        private GameStatus status;
        private GameSettings settings;
        public Systems.HelperSystem helperSystem;

        [Header("UI Reference")]
        public TextMeshProUGUI incomeText;

        [Header("Settings")]
        public string prefix = "초당 수익: ";
        public string suffix = " 냥";

        void Awake()
        {
            status ??= DataHub.Status;
            settings ??= DataHub.Settings;
        }

        void OnEnable()
        {
            if (status != null)
            {
                // 수익에 영향을 주는 값들이 변할 때마다 UI 갱신
                status.OnWigglePowerChanged += UpdateIncomeDisplay;
                status.OnMinSimChanged += UpdateIncomeDisplay;
            }

            var hSystem = helperSystem != null ? helperSystem : Systems.HelperSystem.Instance;
            if (hSystem != null && hSystem.helperStatus != null)
            {
                hSystem.helperStatus.OnHelperUpdated += UpdateIncomeDisplay;
            }

            UpdateIncomeDisplay();
        }

        void OnDisable()
        {
            if (status != null)
            {
                status.OnWigglePowerChanged -= UpdateIncomeDisplay;
                status.OnMinSimChanged -= UpdateIncomeDisplay;
            }

            var hSystem = helperSystem != null ? helperSystem : Systems.HelperSystem.Instance;
            if (hSystem != null && hSystem.helperStatus != null)
            {
                hSystem.helperStatus.OnHelperUpdated -= UpdateIncomeDisplay;
            }
        }
        void Update()
        {
            UpdateIncomeDisplay();
        }

        private void UpdateIncomeDisplay()
        {
            if (incomeText == null || status == null || settings == null) return;

            // EconomySystem과 동일한 공식으로 현재 초당 수익 계산
            float sentimentModifier = CalculateSentimentModifier(status.minSim);

            var hSystem = helperSystem != null ? helperSystem : Systems.HelperSystem.Instance;
            double helperIncome = (hSystem != null) ? hSystem.GetCurrentTotalHelperIncome() : 0;

            // 투자 수익은 완료 시 직접 지급되므로 표시 수익 합산에서 제외
            double totalBaseIncome = settings.baseIncomePerSecond + helperIncome;

            double totalIncomePerSecond = totalBaseIncome * status.wigglePower * sentimentModifier;

            // NumberFormatter 적용
            incomeText.text = $"{prefix}{NumberFormatter.Format(totalIncomePerSecond)}{suffix}";
        }

        // 민심 보정치 계산 (EconomySystem의 로직과 동일)
        private float CalculateSentimentModifier(float minSim)
        {
            if (minSim >= 50f)
                return Mathf.Lerp(1.0f, 1.5f, (minSim - 50f) / 50f);
            else
                return Mathf.Lerp(0.2f, 1.0f, minSim / 50f);
        }
    }
}
