using UnityEngine;
using Wiggle.Data;
using Wiggle.Global;
using System.Collections.Generic;

namespace Wiggle.Systems
{
    public class HelperSystem : MonoBehaviour
    {
        public static HelperSystem Instance { get; private set; }

        [Header("Data References")]
        public HelperData[] allHelpers;

        [Header("Visual Settings")]
        [Tooltip("조력자가 생성되어 배치될 부모 오브젝트")]
        public Transform visualParent;

        // DataHub를 통해 데이터에 접근하도록 프로퍼티화
        public GameStatus gameStatus => DataHub.Status;
        public HelperStatus helperStatus => DataHub.HelperStatus;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }
        }

        void Start()
        {
            if (helperStatus != null && allHelpers != null)
            {
                helperStatus.Initialize(allHelpers.Length, false);
                SpawnExistingVisuals();
                
                // 데이터 리셋 이벤트 구독
                helperStatus.OnDataReset += SpawnExistingVisuals;
            }
        }

        void OnDestroy()
        {
            if (helperStatus != null)
            {
                helperStatus.OnDataReset -= SpawnExistingVisuals;
            }
        }
public void SpawnExistingVisuals()
{
    if (visualParent == null || helperStatus == null || allHelpers == null) return;

    // 1. 기존 비주얼 즉시 완전 삭제 (중복 방지)
    int childCount = visualParent.childCount;
    for (int i = childCount - 1; i >= 0; i--)
    {
        Destroy(visualParent.GetChild(i).gameObject);
    }

    // 2. 레벨이 1 이상인 조력자만 소환
    if (helperStatus.helperLevels != null)
            {
                for (int i = 0; i < helperStatus.helperLevels.Length; i++)
                {
                    if (i < allHelpers.Length && helperStatus.helperLevels[i] >= 1)
                    {
                        SpawnVisual(i);
                    }
                }
            }
        }

        private void SpawnVisual(int index)
        {
            if (visualParent == null || allHelpers[index].visualPrefab == null) return;
            Instantiate(allHelpers[index].visualPrefab, visualParent);
        }

        public bool TryUpgradeHelper(int index)
        {
            if (index < 0 || index >= allHelpers.Length) return false;

            if (index > 0 && helperStatus.GetLevel(index - 1) < 1)
            {
                Debug.LogWarning($"{allHelpers[index - 1].helperName}을(를) 먼저 고용해야 합니다!");
                return false;
            }

            int currentLevel = helperStatus.GetLevel(index);
            double cost = allHelpers[index].GetCost(currentLevel);

            if (gameStatus.money >= cost)
            {
                bool isFirstHire = currentLevel == 0;
                gameStatus.money -= cost;
                helperStatus.helperLevels[index]++;
                
                if (isFirstHire) SpawnVisual(index);

                gameStatus.NotifyMoneyChanged();
                helperStatus.NotifyHelperUpdated();
                
                if (SaveManager.Instance != null && SaveManager.Instance.CurrentSlotIndex != -1)
                {
                    SaveManager.Instance.SaveGame(SaveManager.Instance.CurrentSlotIndex);
                }
                
                return true;
            }
            return false;
        }

        public double GetCurrentTotalHelperIncome()
        {
            if (helperStatus == null || allHelpers == null) return 0;
            return helperStatus.GetTotalPassiveIncome(allHelpers);
        }
    }
}
