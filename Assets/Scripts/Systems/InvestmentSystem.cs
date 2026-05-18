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
        public int LevelAtStart { get; private set; }

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

        public GameStatus gameStatus => DataHub.Status;
        public HelperStatus investmentStatus => DataHub.InvestmentStatus;
        public InvestmentData[] allInvestments;

        private List<ActiveInvestment> activeInvestments = new List<ActiveInvestment>();

        void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }
        }

        void Start()
        {
            if (investmentStatus != null && allInvestments != null)
                investmentStatus.Initialize(allInvestments.Length, false);
            
            // 데이터 리셋 시 진행 중인 투자도 초기화
            if (investmentStatus != null)
            {
                investmentStatus.OnDataReset += () => activeInvestments.Clear();
            }
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

        public bool TryStartInvestment(int index)
        {
            if (index < 0 || index >= allInvestments.Length) return false;
            
            int currentLevel = investmentStatus.GetLevel(index);
            if (currentLevel < 1) return false;

            InvestmentData data = allInvestments[index];
            double cost = data.GetInvestmentCost(currentLevel);

            if (gameStatus.money >= cost)
            {
                gameStatus.money -= cost;
                gameStatus.NotifyMoneyChanged();
                activeInvestments.Add(new ActiveInvestment(index, data, currentLevel));
                return true;
            }
            return false;
        }

        public bool TryUpgradeInvestment(int index)
        {
            if (index < 0 || index >= allInvestments.Length) return false;

            int currentLevel = investmentStatus.GetLevel(index);
            double upgradeCost = allInvestments[index].GetUpgradeCost(Mathf.Max(1, currentLevel));

            if (gameStatus.money >= upgradeCost)
            {
                gameStatus.money -= upgradeCost;
                investmentStatus.helperLevels[index]++;
                
                var sentimentSystem = FindObjectOfType<SentimentSystem>();
                if (sentimentSystem != null)
                    sentimentSystem.AddMinSim(allInvestments[index].purchaseMinSim);
                else
                    gameStatus.AddMinSim(allInvestments[index].purchaseMinSim);
                
                gameStatus.NotifyMoneyChanged();
                investmentStatus.NotifyHelperUpdated();
                
                if (SaveManager.Instance != null && SaveManager.Instance.CurrentSlotIndex != -1)
                {
                    SaveManager.Instance.SaveGame(SaveManager.Instance.CurrentSlotIndex);
                }
                return true;
            }
            return false;
        }

        public double GetCurrentTotalPassiveIncome()
        {
            if (investmentStatus == null || allInvestments == null) return 0;

            double total = 0;
            for (int i = 0; i < allInvestments.Length; i++)
            {
                if (allInvestments[i] == null) continue;
                total += allInvestments[i].GetPassiveIncome(investmentStatus.GetLevel(i));
            }

            return total;
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
                gameStatus.AddMoney(income);
                gameStatus.AddMinSim(data.successMinSim);
            }
            else
            {
                gameStatus.AddMinSim(data.failMinSim);
            }

            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSlotIndex != -1)
            {
                SaveManager.Instance.SaveGame(SaveManager.Instance.CurrentSlotIndex);
            }
        }

        public List<ActiveInvestment> GetActiveInvestments() => activeInvestments;
    }
}
