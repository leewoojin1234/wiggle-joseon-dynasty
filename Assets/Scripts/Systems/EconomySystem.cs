using UnityEngine;
using Wiggle.Data;

namespace Wiggle.Systems
{
    public class EconomySystem : MonoBehaviour
    {
        public GameStatus status;
        public GameSettings settings;

        void Update()
        {
            if (status == null || settings == null) return;

            // 민심 보정치 계산
            float sentimentModifier = CalculateSentimentModifier();

            // 자동 돈 생산 계산
            double currentIncome = settings.baseIncomePerSecond * status.wigglePower * sentimentModifier;
            status.AddMoney(currentIncome * Time.deltaTime);
        }

        float CalculateSentimentModifier()
        {
            float minSim = status.minSim;
            if (minSim >= 50f)
            {
                return Mathf.Lerp(1.0f, 1.5f, (minSim - 50f) / 50f);
            }
            else
            {
                return Mathf.Lerp(0.2f, 1.0f, minSim / 50f);
            }
        }
    }
}
