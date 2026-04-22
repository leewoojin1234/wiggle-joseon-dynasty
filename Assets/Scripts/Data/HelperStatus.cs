using UnityEngine;
using System;
using System.Collections.Generic;

namespace Wiggle.Data
{
    [CreateAssetMenu(fileName = "HelperStatus", menuName = "Wiggle/HelperStatus")]
    public class HelperStatus : ScriptableObject
    {
        // 각 조력자의 현재 레벨을 저장 (ID 또는 인덱스 기반)
        public int[] helperLevels;

        public event Action OnHelperUpdated;

        public void Initialize(int count)
        {
            if (helperLevels == null || helperLevels.Length != count)
            {
                helperLevels = new int[count];
            }
        }

        public int GetLevel(int index)
        {
            if (index < 0 || index >= helperLevels.Length) return 0;
            return helperLevels[index];
        }

        public void UpgradeHelper(int index)
        {
            if (index < 0 || index >= helperLevels.Length) return;
            helperLevels[index]++;
            NotifyHelperUpdated();
        }

        public void NotifyHelperUpdated()
        {
            OnHelperUpdated?.Invoke();
        }

        public double GetTotalPassiveIncome(HelperData[] allHelperData)
        {
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
