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
            UpdateIncomeDisplay();
        }

        void OnDisable()
        {
            if (status != null)
            {
                status.OnWigglePowerChanged -= UpdateIncomeDisplay;
                status.OnMinSimChanged -= UpdateIncomeDisplay;
            }
        }

        private void UpdateIncomeDisplay()
        {
            if (incomeText == null || status == null || settings == null) return;

            // EconomySystem과 동일한 공식으로 현재 초당 수익 계산
            float sentimentModifier = CalculateSentimentModifier(status.minSim);
            double totalIncomePerSecond = settings.baseIncomePerSecond * status.wigglePower * sentimentModifier;

            // 소수점 첫째 자리까지 표시
            incomeText.text = $"{prefix}{totalIncomePerSecond:F1}{suffix}";
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
