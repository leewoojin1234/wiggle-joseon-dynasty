using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Wiggle.Global;

namespace Wiggle.UI
{
    public class TaxButtonHandler : MonoBehaviour, IPointerDownHandler
    {
        [Header("⚖️ Tax Settings")]
        [Tooltip("세금 클릭 시 즉시 획득할 돈양")]
        public double taxIncomeAmount = 1000;
        [Tooltip("세금 클릭 시 민심 감소량 (음수로 입력)")]
        public float minSimPenalty = -10f;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (GameManager.Instance == null) return;

            // 1. 코어 로직 (폭군 버튼)
            // 돈 폭증!
            GameManager.Instance.money += taxIncomeAmount;
            // 민심 급감!
            GameManager.Instance.AddMinSim(minSimPenalty);

            Debug.Log($"[폭군] 세금 징수! 돈 +{taxIncomeAmount}, 민심 {minSimPenalty}");

            // 2. UX 피드백 (화면 흔들림 등)
            // 세금 버튼만의 고유한 타격감(붉은색 이펙트 등)을 여기에 구현하세요.
            // WiggleClickHandler의 이펙트 로직을 복사해서 써도 됩니다.
        }
    }
}
