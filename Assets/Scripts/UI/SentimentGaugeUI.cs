using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wiggle.Data;
using Wiggle.Global;

namespace Wiggle.UI
{
    public class SentimentGaugeUI : MonoBehaviour
    {
        [Header("Data Reference")]
        private GameStatus status;

        [Header("UI Reference")]
        public Slider gaugeSlider;
        public Image fillImage;
        public TextMeshProUGUI valueText;

        [Header("Settings")]
        public Color highSentimentColor = Color.green;
        public Color neutralSentimentColor = Color.white;
        public Color lowSentimentColor = Color.red;

        private void Awake() => status ??= DataHub.Status;

        void Start()
        {
            if (gaugeSlider == null)
                gaugeSlider = GetComponent<Slider>();

            // 민심은 기본적으로 0 ~ 100 범위입니다.
            if (gaugeSlider != null)
            {
                gaugeSlider.minValue = 0f;
                gaugeSlider.maxValue = 100f;
            }
            
            UpdateGauge();
        }

        void OnEnable()
        {
            if (status != null)
            {
                status.OnMinSimChanged += UpdateGauge;
            }
        }

        void OnDisable()
        {
            if (status != null)
            {
                status.OnMinSimChanged -= UpdateGauge;
            }
        }

        private void UpdateGauge()
        {
            if (status == null || gaugeSlider == null) return;

            float currentMinSim = status.minSim;
            gaugeSlider.value = currentMinSim;
            if (valueText != null)
                valueText.text = $"{Mathf.RoundToInt(currentMinSim)}/100";

            // 민심 수치에 따른 색상 변경 피드백
            if (fillImage != null)
            {
                if (currentMinSim >= 70f)
                {
                    fillImage.color = highSentimentColor;
                }
                else if (currentMinSim <= 30f)
                {
                    fillImage.color = lowSentimentColor;
                }
                else
                {
                    fillImage.color = neutralSentimentColor;
                }
            }
        }
    }
}
