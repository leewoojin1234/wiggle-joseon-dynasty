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

        public event Action OnMoneyChanged;
        public event Action OnWigglePowerChanged;
        public event Action OnMinSimChanged;

        public void Initialize(float baseWiggle)
        {
            money = 0;
            wigglePower = baseWiggle;
            minSim = 50.0f;
            
            // 모든 UI에 초기화 알림
            NotifyMoneyChanged();
            OnWigglePowerChanged?.Invoke();
            OnMinSimChanged?.Invoke();
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
    }
}
