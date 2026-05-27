using UnityEngine;
using System.IO;
using System;
using Wiggle.Data;
using Wiggle.Systems;

namespace Wiggle.Global
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }
        public int CurrentSlotIndex { get; set; } = -1;

        private string SavePath(int slotIndex) => Path.Combine(Application.persistentDataPath, $"save_{slotIndex}.json");

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SaveGame(int slotIndex)
        {
            if (slotIndex < 0) return;
            CurrentSlotIndex = slotIndex;
            
            GameStatus status = DataHub.Status;
            HelperStatus helperStatus = DataHub.HelperStatus;
            HelperStatus investStatus = DataHub.InvestmentStatus;
            PermanentStatus perm = DataHub.PermanentStatus;

            SaveData data = new SaveData($"슬롯 {slotIndex + 1}");
            data.lastSaveUtcTicks = DateTime.UtcNow.Ticks;
            data.money = status.money;
            data.minSim = status.minSim;
            data.lastWigglePower = status.wigglePower;
            data.rulerAgeSeconds = 0f;
            data.rulerLifeSpanSeconds = DataHub.Settings != null ? DataHub.Settings.rulerLifeSpanSeconds : 600f;
            data.generationEndingPending = false;
            
            if (helperStatus != null && helperStatus.helperLevels != null) 
                data.helperLevels = (int[])helperStatus.helperLevels.Clone();
            
            if (investStatus != null && investStatus.helperLevels != null) 
                data.investmentLevels = (int[])investStatus.helperLevels.Clone();

            if (perm != null)
            {
                data.kingshipPoints = perm.kingshipPoints;
                data.reincarnationCount = perm.reincarnationCount;
                data.incomeBuffLevel = perm.incomeBuffLevel;
                data.wiggleBuffLevel = perm.wiggleBuffLevel;
                data.sentimentBuffLevel = perm.sentimentBuffLevel;
            }

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath(slotIndex), json);
            Debug.Log($"[SaveManager] 슬롯 {slotIndex} 저장 완료");
        }

        public bool LoadGame(int slotIndex)
        {
            string path = SavePath(slotIndex);
            if (!File.Exists(path)) return false;

            CurrentSlotIndex = slotIndex;

            try
            {
                string json = File.ReadAllText(path);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                double offlineSeconds = 0;
                if (data.lastSaveUtcTicks > 0)
                    offlineSeconds = Math.Max(0, (DateTime.UtcNow.Ticks - data.lastSaveUtcTicks) / (double)TimeSpan.TicksPerSecond);

                // 핵심 수치 복구
                DataHub.Status.money = data.money;
                DataHub.Status.minSim = data.minSim;
                DataHub.Status.wigglePower = data.lastWigglePower > 0 ? data.lastWigglePower : DataHub.Settings.wiggleBase;
                float lifeSpanSeconds = data.rulerLifeSpanSeconds > 0 ? data.rulerLifeSpanSeconds : DataHub.Settings.rulerLifeSpanSeconds;
                DataHub.Status.SetRulerLife(0f, lifeSpanSeconds, false);
                
                // 조력자/투자 레벨 복구
                if (data.helperLevels != null)
                    DataHub.HelperStatus.helperLevels = (int[])data.helperLevels.Clone();
                    
                if (data.investmentLevels != null)
                    DataHub.InvestmentStatus.helperLevels = (int[])data.investmentLevels.Clone();

                // 영구 데이터 복구
                if (DataHub.PermanentStatus != null)
                {
                    DataHub.PermanentStatus.kingshipPoints = data.kingshipPoints;
                    DataHub.PermanentStatus.reincarnationCount = Math.Max(1, data.reincarnationCount);
                    DataHub.PermanentStatus.incomeBuffLevel = data.incomeBuffLevel;
                    DataHub.PermanentStatus.wiggleBuffLevel = data.wiggleBuffLevel;
                    DataHub.PermanentStatus.sentimentBuffLevel = data.sentimentBuffLevel;
                }

                // 알림
                DataHub.Status.NotifyAllChanged(); 
                DataHub.HelperStatus.NotifyDataLoaded();
                DataHub.InvestmentStatus.NotifyDataLoaded();
                PetitionSystem.Instance?.GrantOfflinePetition(offlineSeconds);
                
                Debug.Log($"[SaveManager] 슬롯 {slotIndex} 로드 성공");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 로드 중 오류 발생: {e.Message}");
                return false;
            }
        }

        public SaveData GetSaveInfo(int slotIndex)
        {
            string path = SavePath(slotIndex);
            if (!File.Exists(path)) return null;
            return JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        }

        public void NewGame(int slotIndex)
        {
            CurrentSlotIndex = slotIndex;
            DataHub.ResetAllData();
            SaveGame(slotIndex);
            Debug.Log($"[SaveManager] 슬롯 {slotIndex} 새 통치 시작");
        }

        public void DeleteSave(int slotIndex)
        {
            string path = SavePath(slotIndex);
            if (File.Exists(path))
            {
                File.Delete(path);
                if (CurrentSlotIndex == slotIndex) CurrentSlotIndex = -1;
                Debug.Log($"[SaveManager] 슬롯 {slotIndex} 삭제 완료");
            }
        }

        private void OnApplicationQuit() => AutoSaveInternal();
        private void OnApplicationPause(bool pauseStatus) { if (pauseStatus) AutoSaveInternal(); }

        private void AutoSaveInternal()
        {
            if (CurrentSlotIndex != -1)
            {
                SaveGame(CurrentSlotIndex);
            }
        }
    }
}
