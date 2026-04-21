using UnityEngine;
using System.Collections;

namespace Wiggle.Global
{
    public class GameManager : MonoBehaviour
    {
        // 싱글톤 인스턴스
        public static GameManager Instance;

        [Header("Core Stats")]
        public double money = 0;
        public float wigglePower = 1.0f; // 현재 실룩 지수 (기본 1.0)
        public float minSim = 50.0f;     // 민심 (0 ~ 100)

        [Header("Balancing Settings")]
        [Tooltip("기본 초당 돈 생산량")]
        public double baseIncomePerSecond = 10;
        
        [Tooltip("실룩 지수 기본값")]
        public float wiggleBase = 1.0f;
        [Tooltip("클릭 시 실룩 증가량")]
        public float wiggleBoostPerClick = 0.2f;
        [Tooltip("실룩 최대치")]
        public float wiggleMax = 5.0f;
        [Tooltip("초당 실룩 감소율 (감쇠)")]
        public float wiggleDecayRate = 0.3f;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        void Update()
        {
            // 1. 실룩 지수 자연 감소 (감쇠)
            if (wigglePower > wiggleBase)
            {
                wigglePower -= wiggleDecayRate * Time.deltaTime;
                // 기본값 이하로 떨어지지 않게 방지
                wigglePower = Mathf.Max(wigglePower, wiggleBase);
            }

            // 2. 민심 보정치 계산
            float sentimentModifier = CalculateSentimentModifier();

            // 3. 자동 돈 생산 계산 (핵심 공식)
            // 수익 = (기본 + 자동) x 실룩 배율 x 민심 보정
            // *자동 수익 수단은 아직 구현 안했으므로 기본 수익만 계산
            double currentIncome = baseIncomePerSecond * wigglePower * sentimentModifier;
            money += currentIncome * Time.deltaTime;
        }

        // 민심에 따른 배율 계산 (100 -> x1.5, 50 -> x1.0, 0 -> x0.2)
        float CalculateSentimentModifier()
        {
            if (minSim >= 50f)
            {
                // 50~100 사이: 1.0 ~ 1.5로 선형 보간
                return Mathf.Lerp(1.0f, 1.5f, (minSim - 50f) / 50f);
            }
            else
            {
                // 0~50 사이: 0.2 ~ 1.0으로 선형 보간
                return Mathf.Lerp(0.2f, 1.0f, minSim / 50f);
            }
        }

        // 민심 변화 메서드 (다른 스크립트에서 호출)
        public void AddMinSim(float amount)
        {
            minSim = Mathf.Clamp(minSim + amount, 0f, 100f);
            
            if (minSim <= 20f)
            {
                // TODO: 반란 확률 체크 로직 (계획서 3번)
                Debug.LogWarning("민심 위험! 반란 조짐이 보입니다.");
            }
        }
    }
}
