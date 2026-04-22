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
        public Image fillImage; // 게이지의 색상을 바꾸고 싶을 때 사용

        [Header("Colors")]
        public Color normalColor = Color.white;
        public Color bonusColor = Color.yellow;

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
        }

        void Update()
        {
            if (status == null || settings == null || gaugeSlider == null) return;

            // 2. 현재 실룩 수치를 슬라이더 값에 반영
            float currentPower = status.wigglePower;
            gaugeSlider.value = currentPower;

            // 3. 보너스 상태에 따른 시각적 피드백 (색상 변경)
            if (fillImage != null)
            {
                bool isBonus = currentPower > (settings.wiggleBase + 0.01f);
                fillImage.color = isBonus ? bonusColor : normalColor;
            }

            // 4. (선택 사항) 실룩 수치가 높을 때 게이지 자체를 떨리게 만들기
            if (currentPower > 2.0f) 
            {
                float shakeAmount = (currentPower - 1.0f) * 2.0f; // 수치가 높을수록 더 많이 떨림
                transform.localPosition = new Vector3(
                    Mathf.Sin(Time.time * 20f) * shakeAmount, 
                    Mathf.Cos(Time.time * 20f) * shakeAmount, 
                    0);
            }
            else
            {
                transform.localPosition = Vector3.zero;
            }
        }
    }
}
