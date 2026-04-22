using System;
using UnityEngine;
using Wiggle.Data;
using Wiggle.Global;

namespace Wiggle.Systems
{
    public class EconomySystem : MonoBehaviour
    {
        private GameStatus status;
        private GameSettings settings;
        public HelperSystem helperSystem;

        private void Awake()
        {
            status ??= DataHub.Status;
            settings ??= DataHub.Settings;
        }

        void Update()
        {
            if (status == null || settings == null) return;

            // 민심 보정치 계산
            float sentimentModifier = CalculateSentimentModifier();

            // 총 기본 수익 = 고정 기본 수익 + 조력자 수익 합산
            var hSystem = helperSystem != null ? helperSystem : HelperSystem.Instance;
            double helperIncome = (hSystem != null) ? hSystem.GetCurrentTotalHelperIncome() : 0;
            double totalBaseIncome = settings.baseIncomePerSecond + helperIncome;

            // 자동 돈 생산 계산
            double currentIncome = totalBaseIncome * status.wigglePower * sentimentModifier;
            
            // 이벤트를 발생시키지 않고 데이터만 직접 갱신 (성능 및 UI 깜빡임 방지)
            status.money += currentIncome * Time.deltaTime;
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
