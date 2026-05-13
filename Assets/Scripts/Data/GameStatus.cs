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

        /// <summary>
        /// 완전히 새 게임을 시작할 때 모든 데이터를 0(기본값)으로 초기화합니다.
        /// </summary>
        public void ResetToDefault(float baseWiggle)
        {
            money = 0;
            wigglePower = baseWiggle;
            minSim = 50.0f;
            
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