using UnityEngine;
using Wiggle.Data;
using Wiggle.Global;

namespace Wiggle.Systems
{
    public class InvestmentSystem : MonoBehaviour
    {
        public static InvestmentSystem Instance { get; private set; }

        public GameStatus gameStatus;
        public HelperStatus investmentStatus; // 레벨 저장 구조는 Helper와 공유 가능
        public InvestmentData[] allInvestments;

        void Awake()
        {
            if (Instance == null) Instance = this;
            gameStatus ??= DataHub.Status;
            if (investmentStatus != null && allInvestments != null)
            {
                investmentStatus.Initialize(allInvestments.Length);
            }
        }

        public bool TryInvest(int index)
        {
            if (index < 0 || index >= allInvestments.Length) return false;

            int currentLevel = investmentStatus.GetLevel(index);
            double cost = allInvestments[index].GetCost(currentLevel);

            if (gameStatus.money >= cost)
            {
                gameStatus.money -= cost;
                investmentStatus.helperLevels[index]++; // 레벨 업
                
                // 민심 변화 적용 (농업은 +, 상단은 랜덤 등)
                float bonus = allInvestments[index].minSimBonus;
                if (allInvestments[index].investmentName.Contains("상단"))
                {
                    bonus = Random.Range(-5f, 5f); // 상단은 리스크 존재
                }
                gameStatus.AddMinSim(bonus);

                gameStatus.NotifyMoneyChanged();
                investmentStatus.NotifyHelperUpdated();
                return true;
            }
            return false;
        }

        public double GetTotalInvestmentIncome()
        {
            if (investmentStatus == null) return 0;
            double total = 0;
            for (int i = 0; i < allInvestments.Length; i++)
            {
                total += allInvestments[i].GetIncome(investmentStatus.GetLevel(i));
            }
            return total;
        }
    }
}
