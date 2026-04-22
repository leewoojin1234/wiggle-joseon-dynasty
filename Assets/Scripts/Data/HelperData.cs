using UnityEngine;

namespace Wiggle.Data
{
    [CreateAssetMenu(fileName = "HelperData", menuName = "Wiggle/HelperData")]
    public class HelperData : ScriptableObject
    {
        public string helperName;
        public double baseCost;
        public double baseIncome;
        [Tooltip("레벨업 시 가격 상승 지수 (보통 1.15)")]
        public float costMultiplier = 1.15f;
        
        public double GetCost(int currentLevel)
        {
            return baseCost * Mathf.Pow(costMultiplier, currentLevel);
        }

        public double GetTotalIncome(int currentLevel)
        {
            return baseIncome * currentLevel;
        }
    }
}
