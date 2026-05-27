using UnityEngine;
using System;

namespace Wiggle.Data
{
    [CreateAssetMenu(fileName = "GameStatus", menuName = "Wiggle/GameStatus")]
    public class GameStatus : ScriptableObject
    {
        public double money;
        public float wigglePower;
        public float minSim;
        public float rulerAgeSeconds;
        public float rulerLifeSpanSeconds;
        public bool isGenerationEndingPending;

        public event Action OnMoneyChanged;
        public event Action OnWigglePowerChanged;
        public event Action OnMinSimChanged;
        public event Action OnRulerLifeChanged;
        public event Action OnGenerationEndingChanged;

        /// <summary>
        /// 완전히 새 게임을 시작할 때 모든 데이터를 0(기본값)으로 초기화합니다.
        /// </summary>
        public void ResetToDefault(float baseWiggle, float lifeSpanSeconds = 600f)
        {
            money = 0;
            wigglePower = baseWiggle;
            minSim = 50.0f;
            rulerAgeSeconds = 0f;
            rulerLifeSpanSeconds = Mathf.Max(1f, lifeSpanSeconds);
            isGenerationEndingPending = false;
            
            NotifyAllChanged();
        }

        /// <summary>
        /// 데이터 변경 없이, 연결된 모든 UI에게 현재 값을 다시 그리라고 알립니다. (로드 시 사용)
        /// </summary>
        public void NotifyAllChanged()
        {
            NotifyMoneyChanged();
            OnWigglePowerChanged?.Invoke();
            OnMinSimChanged?.Invoke();
            OnRulerLifeChanged?.Invoke();
            OnGenerationEndingChanged?.Invoke();
        }

        public void AddMoney(double amount)
        {
            money += amount;
            NotifyMoneyChanged();
        }

        public void NotifyMoneyChanged()
        {
            OnMoneyChanged?.Invoke();
        }

        public void AddWiggle(float amount, float max, float min)
        {
            wigglePower = Mathf.Clamp(wigglePower + amount, min, max);
            OnWigglePowerChanged?.Invoke();
        }

        public void SetWiggle(float value)
        {
            wigglePower = value;
            OnWigglePowerChanged?.Invoke();
        }

        public void AddMinSim(float amount)
        {
            minSim = Mathf.Clamp(minSim + amount, 0f, 100f);
            OnMinSimChanged?.Invoke();
        }

        public bool AdvanceRulerLife(float seconds, float fallbackLifeSpanSeconds)
        {
            if (isGenerationEndingPending) return true;

            if (rulerLifeSpanSeconds <= 0f)
                rulerLifeSpanSeconds = Mathf.Max(1f, fallbackLifeSpanSeconds);

            rulerAgeSeconds = Mathf.Clamp(rulerAgeSeconds + Mathf.Max(0f, seconds), 0f, rulerLifeSpanSeconds);
            OnRulerLifeChanged?.Invoke();

            if (rulerAgeSeconds < rulerLifeSpanSeconds) return false;

            BeginGenerationEnding();
            return true;
        }

        public void BeginGenerationEnding()
        {
            if (isGenerationEndingPending) return;

            isGenerationEndingPending = true;
            OnGenerationEndingChanged?.Invoke();
        }

        public void SetRulerLife(float ageSeconds, float lifeSpanSeconds, bool endingPending)
        {
            rulerLifeSpanSeconds = Mathf.Max(1f, lifeSpanSeconds);
            rulerAgeSeconds = Mathf.Clamp(ageSeconds, 0f, rulerLifeSpanSeconds);
            isGenerationEndingPending = endingPending || rulerAgeSeconds >= rulerLifeSpanSeconds;
            OnRulerLifeChanged?.Invoke();
            OnGenerationEndingChanged?.Invoke();
        }
    }
}
