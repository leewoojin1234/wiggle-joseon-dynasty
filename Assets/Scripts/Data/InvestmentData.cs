using UnityEngine;

namespace Wiggle.Data
{
    public enum InvestmentMeansType
    {
        Agriculture,
        RealEstate,
        Merchant,
        Relief
    }

    [CreateAssetMenu(fileName = "InvestmentData", menuName = "Wiggle/InvestmentData")]
    public class InvestmentData : ScriptableObject
    {
        [Header("기본 정보")]
        public string investmentName;
        public string description;
        public InvestmentMeansType meansType = InvestmentMeansType.Agriculture;

        [Header("자동 생산 수단")]
        public double basePassiveIncome;
        public double passiveIncomePerLevel;
        public float purchaseMinSim;
        public float passiveIncomeGrowthPerLevel;

        [Header("기본 수치 (레벨 1 기준)")]
        public double baseCost;          // 투자 비용
        public float baseDuration;      // 소요 시간
        public double baseSuccessIncome; // 성공 수익
        public float baseSuccessRate;   // 성공 확률 (0~1)

        [Header("레벨업 상승치")]
        public double upgradeCostBase;    // 레벨업에 필요한 기본 비용
        public float upgradeCostMultiplier = 1.15f; // 레벨업 비용 상승률
        
        public double incomePerLevel;     // 레벨당 추가 수익
        public float durationReducePerLevel; // 레벨당 단축 시간
        public float successRatePerLevel; // 레벨당 추가 확률

        [Header("민심 (고정 또는 레벨 영향)")]
        public float successMinSim;
        public float failMinSim;

        // --- 레벨에 따른 실시간 수치 계산 함수들 ---

        public double GetInvestmentCost(int level) => baseCost; 
        
        public double GetUpgradeCost(int level) => upgradeCostBase * Mathf.Pow(upgradeCostMultiplier, level - 1);

        public float GetDuration(int level) 
        {
            float time = baseDuration - (durationReducePerLevel * (level - 1));
            return Mathf.Max(time, 1f); // 최소 1초는 걸리도록 제한
        }

        public double GetSuccessIncome(int level) => baseSuccessIncome + (incomePerLevel * (level - 1));

        public double GetPassiveIncome(int level)
        {
            if (level <= 0) return 0;

            double linearIncome = basePassiveIncome + (passiveIncomePerLevel * (level - 1));
            float growthMultiplier = passiveIncomeGrowthPerLevel > 0
                ? Mathf.Pow(1f + passiveIncomeGrowthPerLevel, level - 1)
                : 1f;

            return linearIncome * level * growthMultiplier;
        }

        public float GetSuccessRate(int level)
        {
            float rate = baseSuccessRate + (successRatePerLevel * (level - 1));
            return Mathf.Min(rate, 1f); // 확률은 100%를 넘지 않음
        }
    }
}
