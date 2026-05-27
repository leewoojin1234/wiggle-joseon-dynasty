using UnityEngine;

namespace Wiggle.Data
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Wiggle/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Economy Settings")]
        public double baseIncomePerSecond = 10;
        public double clickBaseIncome = 1;

        [Header("Wiggle Settings")]
        public float wiggleBase = 1.0f;
        public float wiggleBoostPerClick = 0.2f;
        public float wiggleMax = 5.0f;
        public float wiggleDecayRate = 0.3f;

        [Header("Sentiment Pressure")]
        public float overWiggleThreshold = 3.5f;
        public float overWiggleSentimentLossPerSecond = 0.25f;

        [Header("Tax Settings")]
        public float taxIncomeMultiplier = 100f;
        public float taxMinSimPenalty = -10f;

        [Header("Ruler Life")]
        [Tooltip("한 왕의 통치 수명입니다. MVP에서는 실제 초 단위로 두고 밸런싱합니다.")]
        public float rulerLifeSpanSeconds = 600f;
    }
}
