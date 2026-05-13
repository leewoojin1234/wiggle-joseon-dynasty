using UnityEngine;
using Wiggle.Data;
using Wiggle.Global;

namespace Wiggle.Systems
{
    public class SentimentSystem : MonoBehaviour
    {
        public GameStatus status => DataHub.Status;

        public void AddMinSim(float amount)
        {
            if (status == null) return;
            
            // 영구 버프 적용 (민심 감소 완화)
            if (amount < 0 && DataHub.PermanentStatus != null)
            {
                amount *= DataHub.PermanentStatus.GetSentimentPenaltyMultiplier();
            }

            status.AddMinSim(amount);
            CheckRebellion();
        }

        private void CheckRebellion()
        {
            float currentMinSim = status.minSim;

            if (currentMinSim <= 0f)
            {
                // 민심 0이면 확정 반란
                ReincarnationManager.Instance?.TriggerRebellion();
            }
            else if (currentMinSim <= 20f)
            {
                // 민심 20 이하부터 확률적 반란 (민심이 낮을수록 확률 증가)
                // 예: 민심 10이면 약 10% 확률로 반란
                float rebellionChance = (20f - currentMinSim) / 100f; 
                if (Random.Range(0f, 1f) < rebellionChance)
                {
                    ReincarnationManager.Instance?.TriggerRebellion();
                }
                else
                {
                    Debug.LogWarning($"민심이 흉흉합니다! (반란 확률: {rebellionChance * 100:F1}%)");
                }
            }
        }
    }
}
