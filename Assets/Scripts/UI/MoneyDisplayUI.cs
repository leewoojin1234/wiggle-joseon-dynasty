using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 필요
using Wiggle.Data;

namespace Wiggle.UI
{
    public class MoneyDisplayUI : MonoBehaviour
    {
        [Header("Data Reference")]
        public GameStatus status;

        [Header("UI Reference")]
        [Tooltip("돈을 표시할 TextMeshProUGUI 컴포넌트")]
        public TextMeshProUGUI moneyText;

        [Header("Settings")]
        [Tooltip("숫자 앞에 붙일 접두어 (예: ₩, $)")]
        public string prefix = "냥: ";
        [Tooltip("숫자 뒤에 붙일 접미어")]
        public string suffix = "";

        void OnEnable()
        {
            if (status != null)
            {
                // 데이터 변경 시 호출될 이벤트 구독
                status.OnMoneyChanged += UpdateMoneyDisplay;
            }
            // 초기 화면 업데이트
            UpdateMoneyDisplay();
        }

        void OnDisable()
        {
            if (status != null)
            {
                // 오브젝트가 비활성화될 때 이벤트 구독 해제 (메모리 누수 방지)
                status.OnMoneyChanged -= UpdateMoneyDisplay;
            }
        }

        private void UpdateMoneyDisplay()
        {
            if (moneyText == null || status == null) return;

            // 돈 단위를 정수로 표현하거나, 특정 포맷(예: 천 단위 콤마)으로 출력
            // "N0"는 소수점 없이 천 단위 콤마를 추가하는 포맷입니다.
            moneyText.text = $"{prefix}{status.money:N0}{suffix}";
        }
    }
}
