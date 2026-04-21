using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Wiggle.Data;
using Wiggle.Systems;

namespace Wiggle.UI
{
    public class TaxButtonHandler : MonoBehaviour, IPointerDownHandler
    {
        [Header("Systems & Data")]
        public GameStatus status;
        public SentimentSystem sentimentSystem;

        [Header("Tax Settings")]
        [Tooltip("세금 클릭 시 즉시 획득할 돈양")]
        public double taxIncomeAmount = 1000;
        [Tooltip("세금 클릭 시 민심 감소량 (음수로 입력)")]
        public float minSimPenalty = -10f;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (status == null || sentimentSystem == null) return;

            // 1. 코어 로직 (폭군 버튼)
            // 돈 폭증!
            status.AddMoney(taxIncomeAmount);
            // 민심 급감!
            sentimentSystem.AddMinSim(minSimPenalty);

            Debug.Log($"[폭군] 세금 징수! 돈 +{taxIncomeAmount}, 민심 {minSimPenalty}");
        }
    }
}
