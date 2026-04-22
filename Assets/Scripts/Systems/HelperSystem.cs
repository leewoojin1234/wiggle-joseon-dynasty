using UnityEngine;
using Wiggle.Data;
using Wiggle.Global;

namespace Wiggle.Systems
{
    public class HelperSystem : MonoBehaviour
    {
        public static HelperSystem Instance { get; private set; }

        [Header("Data References")]
        public GameStatus gameStatus;
        public HelperStatus helperStatus;
        public HelperData[] allHelpers;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            gameStatus ??= DataHub.Status;
            
            if (helperStatus != null && allHelpers != null)
            {
                helperStatus.Initialize(allHelpers.Length);
            }
        }

        // 특정 조력자를 고용하거나 업그레이드 시도
        public bool TryUpgradeHelper(int index)
        {
            if (index < 0 || index >= allHelpers.Length) return false;

            // 1. 순차 고용 체크: 이전 단계 조력자가 최소 레벨 1이어야 함
            if (index > 0 && helperStatus.GetLevel(index - 1) < 1)
            {
                Debug.LogWarning($"{allHelpers[index - 1].helperName}을(를) 먼저 고용해야 합니다!");
                return false;
            }

            // 2. 가격 확인
            int currentLevel = helperStatus.GetLevel(index);
            double cost = allHelpers[index].GetCost(currentLevel);

            if (gameStatus.money >= cost)
            {
                // 3. 지불 및 레벨업
                gameStatus.money -= cost; // 직접 차감 (이벤트 발생 X)
                helperStatus.helperLevels[index]++; // 직접 레벨업 (이벤트 발생 X)
                
                // 4. 모든 처리가 끝난 후 한 번만 알림
                gameStatus.NotifyMoneyChanged();
                helperStatus.NotifyHelperUpdated();
                
                Debug.Log($"{allHelpers[index].helperName} 레벨업! 현재 레벨: {helperStatus.GetLevel(index)}");
                return true;
            }
            else
            {
                Debug.LogWarning("돈이 부족합니다!");
                return false;
            }
        }

        public double GetCurrentTotalHelperIncome()
        {
            if (helperStatus == null || allHelpers == null) return 0;
            return helperStatus.GetTotalPassiveIncome(allHelpers);
        }
    }
}
