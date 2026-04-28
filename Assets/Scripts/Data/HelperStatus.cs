using UnityEngine;
using System;

namespace Wiggle.Data
{
    [CreateAssetMenu(fileName = "HelperStatus", menuName = "Wiggle/HelperStatus")]
    public class HelperStatus : ScriptableObject
    {
        // 각 조력자/투자의 현재 레벨을 저장
        public int[] helperLevels;

        public event Action OnHelperUpdated;
        public event Action OnDataReset; // 데이터가 완전히 초기화되었음을 알리는 이벤트

        public void Initialize(int count, bool forceReset = false)
        {
            // 에디터 영속성 문제 방지: 배열이 없거나 개수가 다르거나 강제 리셋인 경우 초기화
            if (forceReset || helperLevels == null || helperLevels.Length != count)
            {
                helperLevels = new int[count];
                OnDataReset?.Invoke();
                NotifyHelperUpdated();
            }
        }

        /// <summary>
        /// 데이터를 완전히 초기화합니다. (New Game 용)
        /// </summary>
        public void ResetData()
        {
            if (helperLevels != null)
            {
                for (int i = 0; i < helperLevels.Length; i++)
                {
                    helperLevels[i] = 0;
                }
            }
            NotifyDataLoaded();
        }

        public void NotifyDataLoaded()
        {
            OnDataReset?.Invoke();
            NotifyHelperUpdated();
        }

        public int GetLevel(int index)
        {
            if (helperLevels == null || index < 0 || index >= helperLevels.Length) return 0;
            return helperLevels[index];
        }

        public void UpgradeHelper(int index)
        {
            if (helperLevels == null || index < 0 || index >= helperLevels.Length) return;
            helperLevels[index]++;
            NotifyHelperUpdated();
        }

        public void NotifyHelperUpdated()
        {
            OnHelperUpdated?.Invoke();
        }

        public double GetTotalPassiveIncome(HelperData[] allHelperData)
        {
            if (helperLevels == null) return 0;
            double total = 0;
            for (int i = 0; i < helperLevels.Length; i++)
            {
                if (i < allHelperData.Length)
                {
                    total += allHelperData[i].GetTotalIncome(helperLevels[i]);
                }
            }
            return total;
        }
    }
}
