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

            var hSystem = helperSystem != null ? helperSystem : Systems.HelperSystem.Instance;
            double totalIncomePerSecond = Systems.EconomyFormula.GetIncomePerSecond(
                status,
                settings,
                hSystem,
                Systems.InvestmentSystem.Instance);

            // NumberFormatter 적용
            incomeText.text = $"{prefix}{NumberFormatter.Format(totalIncomePerSecond)}{suffix}";
        }
    }
}
