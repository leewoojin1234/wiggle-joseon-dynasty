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
                // 방치형 클리커에서는 저민심이 즉시 랜덤 처벌이 되면 플레이 감각이 무너집니다.
                // 20 이하는 경제 효율 하락과 경고 상태로만 두고, 반란은 민심 0에서 확정 발생시킵니다.
                Debug.LogWarning("민심이 흉흉합니다! 수익 효율이 크게 떨어졌고, 민심 0이 되면 반란이 발생합니다.");
            }
        }
    }
}
