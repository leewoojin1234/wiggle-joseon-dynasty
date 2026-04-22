using UnityEngine;

namespace Wiggle.Data
{
    [CreateAssetMenu(fileName = "InvestmentData", menuName = "Wiggle/InvestmentData")]
    public class InvestmentData : ScriptableObject
    {
        public string investmentName;
        public string description;
        public double baseCost;
        public double baseIncome;
        public float minSimBonus; // 구매 시 민심 변화량
        public float costMultiplier = 1.2f;

        public double GetCost(int level) => baseCost * Mathf.Pow(costMultiplier, level);
        public double GetIncome(int level) => baseIncome * level;
    }
}
