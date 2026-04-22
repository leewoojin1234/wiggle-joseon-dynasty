using System.Collections.Generic;
using UnityEngine;
using Wiggle.Data;
using Wiggle.Global;

namespace Wiggle.Systems
{
    public class ActiveInvestment
    {
        public int Index { get; private set; }
        public InvestmentData Data { get; private set; }
        public float RemainingTime { get; set; }
        public int LevelAtStart { get; private set; } // 시작 시점의 레벨 저장

        public ActiveInvestment(int index, InvestmentData data, int level)
        {
            Index = index;
            Data = data;
            LevelAtStart = level;
            RemainingTime = data.GetDuration(level);
        }
    }

    public class InvestmentSystem : MonoBehaviour
    {
        public static InvestmentSystem Instance { get; private set; }

        public GameStatus gameStatus;
        public HelperStatus investmentStatus; // 레벨 저장을 위한 객체 (기존 구조 활용)
        public InvestmentData[] allInvestments;

        private List<ActiveInvestment> activeInvestments = new List<ActiveInvestment>();

        void Awake()
        {
            if (Instance == null) Instance = this;
            gameStatus ??= DataHub.Status;
            
            // 투자 항목 수만큼 레벨 배열 초기화 (처음 시작 시)
            if (investmentStatus != null && allInvestments != null)
                investmentStatus.Initialize(allInvestments.Length);
        }

        void Update()
        {
            for (int i = activeInvestments.Count - 1; i >= 0; i--)
            {
                activeInvestments[i].RemainingTime -= Time.deltaTime;
                if (activeInvestments[i].RemainingTime <= 0)
                {
                    CompleteInvestment(activeInvestments[i]);
                    activeInvestments.RemoveAt(i);
                }
            }
        }

        // [기능 1] 투자 시작 (보내기)
        public bool TryStartInvestment(int index)
        {
            if (index < 0 || index >= allInvestments.Length) return false;
            
            int currentLevel = investmentStatus.GetLevel(index);
            if (currentLevel < 1) 
            {
                Debug.LogWarning("먼저 항목을 해금(레벨업)해야 합니다.");
                return false;
            }

            InvestmentData data = allInvestments[index];
            double cost = data.GetInvestmentCost(currentLevel);

            if (gameStatus.money >= cost)
            {
                gameStatus.money -= cost;
                gameStatus.NotifyMoneyChanged();

                activeInvestments.Add(new ActiveInvestment(index, data, currentLevel));
                Debug.Log($"{data.investmentName} (Lv.{currentLevel}) 투자 시작!");
                return true;
            }
            return false;
        }

        // [기능 2] 투자 항목 레벨업 (강화)
        public bool TryUpgradeInvestment(int index)
        {
            if (index < 0 || index >= allInvestments.Length) return false;

            int currentLevel = investmentStatus.GetLevel(index);
            // 첫 해금 시에는 레벨 0 -> 1이므로 -1 대신 적절한 로직 필요 (여기선 제공된 공식 유지)
            double upgradeCost = allInvestments[index].GetUpgradeCost(Mathf.Max(1, currentLevel));

            if (gameStatus.money >= upgradeCost)
            {
                gameStatus.money -= upgradeCost;
                investmentStatus.helperLevels[index]++; // 레벨 상승
                
                gameStatus.NotifyMoneyChanged();
                investmentStatus.NotifyHelperUpdated(); // UI 갱신용 알림
                Debug.Log($"{allInvestments[index].investmentName} 레벨업! 현재 Lv.{investmentStatus.GetLevel(index)}");
                return true;
            }
            
            Debug.Log("레벨업 비용이 부족합니다.");
            return false;
        }

        private void CompleteInvestment(ActiveInvestment activeInv)
        {
            InvestmentData data = activeInv.Data;
            int lv = activeInv.LevelAtStart;

            float successRate = data.GetSuccessRate(lv);
            bool isSuccess = Random.Range(0f, 1f) <= successRate;

            if (isSuccess)
            {
                double income = data.GetSuccessIncome(lv);
                gameStatus.AddMoney(income); // AddMoney 사용 시 Notify 자동 포함
                gameStatus.AddMinSim(data.successMinSim);
                Debug.Log($"<color=green>[성공]</color> {data.investmentName} 수익 {income} 획득!");
            }
            else
            {
                gameStatus.AddMinSim(data.failMinSim);
                Debug.Log($"<color=red>[실패]</color> {data.investmentName} 성과 없음.");
            }
        }

        public List<ActiveInvestment> GetActiveInvestments() => activeInvestments;
    }
}
