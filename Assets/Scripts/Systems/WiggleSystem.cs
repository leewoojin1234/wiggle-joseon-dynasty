using UnityEngine;
using Wiggle.Data;
using Wiggle.Global;

namespace Wiggle.Systems
{
    public class WiggleSystem : MonoBehaviour
    {
        public GameStatus status => DataHub.Status;
        public GameSettings settings => DataHub.Settings;
        public SentimentSystem sentimentSystem;

        void Start()
        {
            if (status != null && settings != null)
            {
                // status.Initialize는 DataHub에서 관리하되, 안전장치로 유지
                if (status.wigglePower < settings.wiggleBase)
                    status.SetWiggle(settings.wiggleBase);
            }
        }

        void Update()
        {
            if (status == null || settings == null) return;

            // 실룩 지수 자연 감소 (감쇠)
            if (status.wigglePower > settings.wiggleBase)
            {
                float decayRate = settings.wiggleDecayRate;

                // 영구 버프 적용 (감소율 경감)
                if (DataHub.PermanentStatus != null)
                {
                    decayRate *= (1.0f - DataHub.PermanentStatus.GetWiggleDecayReduction());
                }

                float newPower = status.wigglePower - (decayRate * Time.deltaTime);
                status.SetWiggle(Mathf.Max(newPower, settings.wiggleBase));
            }

            if (status.wigglePower > settings.overWiggleThreshold && settings.overWiggleSentimentLossPerSecond > 0)
            {
                var sSystem = sentimentSystem != null ? sentimentSystem : FindObjectOfType<SentimentSystem>();
                sSystem?.AddMinSim(-settings.overWiggleSentimentLossPerSecond * Time.deltaTime);
            }
        }

        public void BoostWiggle()
        {
            if (status == null || settings == null) return;
            status.AddWiggle(settings.wiggleBoostPerClick, settings.wiggleMax, settings.wiggleBase);
        }
    }
}
