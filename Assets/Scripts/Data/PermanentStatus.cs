using UnityEngine;

namespace Wiggle.Data
{
    [CreateAssetMenu(fileName = "PermanentStatus", menuName = "Wiggle/PermanentStatus")]
    public class PermanentStatus : ScriptableObject
    {
        [Header("Prestige Currency")]
        public long kingshipPoints; // 왕권 포인트
        public int reincarnationCount; // 통치 대수 (1대, 2대...)

        [Header("Permanent Buffs (Levels)")]
        public int incomeBuffLevel;      // 수익 강화 레벨
        public int wiggleBuffLevel;      // 실룩 유지 강화 레벨
        public int sentimentBuffLevel;   // 민심 보호 강화 레벨

        /// <summary>
        /// 모든 영구 데이터를 초기화합니다 (전체 초기화용).
        /// </summary>
        public void ResetAll()
        {
            kingshipPoints = 0;
            reincarnationCount = 1;
            incomeBuffLevel = 0;
            wiggleBuffLevel = 0;
            sentimentBuffLevel = 0;
        }

        // --- 보너스 계산 함수들 ---

        public float GetIncomeMultiplier() => 1.0f + (incomeBuffLevel * 0.1f); // 레벨당 10% 증가
        public float GetWiggleDecayReduction() => Mathf.Clamp(wiggleBuffLevel * 0.05f, 0f, 0.5f); // 최대 50% 감소
        public float GetSentimentPenaltyMultiplier() => Mathf.Max(0.5f, 1.0f - (sentimentBuffLevel * 0.05f)); // 최대 50% 경감
    }
}
