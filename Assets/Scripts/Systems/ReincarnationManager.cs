using UnityEngine;
using Wiggle.Data;
using Wiggle.Global;
using System;

namespace Wiggle.Systems
{
    public class ReincarnationManager : MonoBehaviour
    {
        public static ReincarnationManager Instance { get; private set; }

        public event Action OnRebellionStarted;
        public event Action OnReincarnationCompleted;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }
        }

        /// <summary>
        /// 반란 발생! (패널티와 함께 강제 환생)
        /// </summary>
        public void TriggerRebellion()
        {
            OnRebellionStarted?.Invoke();
            
            // 1. 보상 계산 (패널티로 인해 정상 환생보다 적게 줌)
            long rewardPoints = CalculatePrestigePoints(true);
            
            // 2. 영구 데이터 업데이트
            var perm = DataHub.PermanentStatus;
            perm.kingshipPoints += rewardPoints;
            perm.reincarnationCount++;

            // 3. 현재 데이터 리셋
            DataHub.ResetAllData();
            
            // 4. 세이브
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSlotIndex != -1)
            {
                SaveManager.Instance.SaveGame(SaveManager.Instance.CurrentSlotIndex);
            }

            Debug.Log($"<color=red>[반란]</color> 백성들이 궐기로 왕조가 교체되었습니다. 포인트 {rewardPoints} 획득.");
            OnReincarnationCompleted?.Invoke();
        }

        /// <summary>
        /// 평화로운 선위 (목표 달성 시 정상 환생)
        /// </summary>
        public void TriggerNormalReincarnation()
        {
            long rewardPoints = CalculatePrestigePoints(false);
            
            var perm = DataHub.PermanentStatus;
            perm.kingshipPoints += rewardPoints;
            perm.reincarnationCount++;

            DataHub.ResetAllData();
            
            if (SaveManager.Instance != null && SaveManager.Instance.CurrentSlotIndex != -1)
                SaveManager.Instance.SaveGame(SaveManager.Instance.CurrentSlotIndex);

            Debug.Log($"<color=green>[선위]</color> 안정적으로 다음 대 왕에게 왕위를 물려주었습니다. 포인트 {rewardPoints} 획득.");
            OnReincarnationCompleted?.Invoke();
        }

        private long CalculatePrestigePoints(bool isRebellion)
        {
            // 돈의 로그값이나 특정 기준치를 바탕으로 포인트 계산
            double money = DataHub.Status.money;
            long points = (long)(Math.Log10(Math.Max(1, money)) * 10);
            
            // 반란 시 50%만 획득
            if (isRebellion) points /= 2;
            
            return Math.Max(1, points);
        }

        public long PreviewPrestigePoints(bool isRebellion)
        {
            return CalculatePrestigePoints(isRebellion);
        }
    }
}
