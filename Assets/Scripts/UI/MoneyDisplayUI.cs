using UnityEngine;
using TMPro;
using Wiggle.Data;
using Wiggle.Global;

namespace Wiggle.UI
{
    public class MoneyDisplayUI : MonoBehaviour
    {
        [Header("Data Reference")]
        private GameStatus status;

        [Header("UI Reference")]
        [Tooltip("돈을 표시할 TextMeshProUGUI 컴포넌트")]
        public TextMeshProUGUI moneyText;

        [Header("Settings")]
        [Tooltip("숫자 앞에 붙일 접두어 (예: ₩, $)")]
        public string prefix = "냥: ";
        [Tooltip("숫자 뒤에 붙일 접미어")]
        public string suffix = "";

        void Awake() => status ??= DataHub.Status;

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
void Update()
{
    UpdateMoneyDisplay();
}

private void UpdateMoneyDisplay()
{
    if (moneyText == null || status == null) return;

    // NumberFormatter 적용
    moneyText.text = $"{prefix}{NumberFormatter.Format(status.money)}{suffix}";
}

    }
}
