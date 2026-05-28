using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wiggle.Data;
using Wiggle.Global;

namespace Wiggle.UI
{
    public class WiggleGaugeUI : MonoBehaviour
    {
        [Header("Data Reference")]
        private GameStatus status;
        private GameSettings settings;

        [Header("UI Reference")]
        public Slider gaugeSlider;
        public TextMeshProUGUI baseValueText;
        public TextMeshProUGUI maxValueText;
        
        private void Awake()
        {
            status ??= DataHub.Status;
            settings ??= DataHub.Settings;
        }
        
        void Start()
        {
            if (gaugeSlider == null)
                gaugeSlider = GetComponent<Slider>();

            // 1. 슬라이더의 최소/최대값을 설정에 맞게 초기화
            if (settings != null)
            {
                gaugeSlider.minValue = settings.wiggleBase;
                gaugeSlider.maxValue = settings.wiggleMax;
            }

            UpdateRangeTexts();
        }

        void Update()
        {
            if (status == null || settings == null || gaugeSlider == null) return;

            // 2. 현재 실룩 수치를 슬라이더 값에 반영
            float currentPower = status.wigglePower;
            gaugeSlider.value = currentPower;
        }

        private void UpdateRangeTexts()
        {
            if (settings == null) return;

            if (baseValueText != null)
                baseValueText.text = FormatMultiplier(settings.wiggleBase);

            if (maxValueText != null)
                maxValueText.text = FormatMultiplier(settings.wiggleMax);
        }

        private string FormatMultiplier(float value)
        {
            return $"{value:0.0}x";
        }
    }
}
