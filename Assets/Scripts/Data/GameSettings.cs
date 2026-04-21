using UnityEngine;

namespace Wiggle.Data
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Wiggle/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Economy Settings")]
        public double baseIncomePerSecond = 10;

        [Header("Wiggle Settings")]
        public float wiggleBase = 1.0f;
        public float wiggleBoostPerClick = 0.2f;
        public float wiggleMax = 5.0f;
        public float wiggleDecayRate = 0.3f;
    }
}
