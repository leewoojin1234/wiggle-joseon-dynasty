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

        /// <summary>
        /// 현재 게임 상태를 파일로 저장합니다.
        /// </summary>
        public void SaveGame(int slotIndex)
        {
            if (slotIndex < 0) return;
            CurrentSlotIndex = slotIndex;
            
            GameStatus status = DataHub.Status;
            HelperStatus helperStatus = DataHub.HelperStatus;
            HelperStatus investStatus = DataHub.InvestmentStatus;

            SaveData data = new SaveData($"슬롯 {slotIndex + 1}");
            data.money = status.money;
            data.minSim = status.minSim;
            // 실룩 지수도 저장하여 일관성 유지
            data.lastWigglePower = status.wigglePower;
            
            // 데이터 복제 (참조 오염 방지)
            if (helperStatus != null && helperStatus.helperLevels != null) 
                data.helperLevels = (int[])helperStatus.helperLevels.Clone();
            
            if (investStatus != null && investStatus.helperLevels != null) 
                data.investmentLevels = (int[])investStatus.helperLevels.Clone();

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath(slotIndex), json);
            Debug.Log($"[SaveManager] 슬롯 {slotIndex} 저장 완료 (위치: {SavePath(slotIndex)})");
        }

        /// <summary>
        /// 파일을 읽어 게임 상태에 적용합니다.
        /// </summary>
        public bool LoadGame(int slotIndex)
        {
            string path = SavePath(slotIndex);
            if (!File.Exists(path)) return false;

            CurrentSlotIndex = slotIndex;

            try
            {
                string json = File.ReadAllText(path);
                SaveData data = JsonUtility.FromJson<SaveData>(json);

                // 1. 핵심 수치 복구
                DataHub.Status.money = data.money;
                DataHub.Status.minSim = data.minSim;
                DataHub.Status.wigglePower = data.lastWigglePower > 0 ? data.lastWigglePower : DataHub.Settings.wiggleBase;
                
                // 2. 조력자/투자 레벨 복구
                if (data.helperLevels != null)
                    DataHub.HelperStatus.helperLevels = (int[])data.helperLevels.Clone();
                    
                if (data.investmentLevels != null)
                    DataHub.InvestmentStatus.helperLevels = (int[])data.investmentLevels.Clone();

                // 3. 모든 시스템에 데이터 변경 알림 (비주얼 및 UI 갱신 트리거)
                DataHub.Status.Initialize(DataHub.Status.wigglePower); 
                DataHub.HelperStatus.NotifyDataLoaded();
                DataHub.InvestmentStatus.NotifyDataLoaded();
                
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

        /// <summary>
        /// 완전히 새로운 데이터로 슬롯을 초기화하고 게임을 시작합니다.
        /// </summary>
        public void NewGame(int slotIndex)
        {
            CurrentSlotIndex = slotIndex;

            // 1. 모든 데이터 에셋 초기화
            DataHub.ResetAllData();

            // 2. 초기화된 상태를 즉시 파일로 덮어쓰기 (슬롯 생성/초기화)
            SaveGame(slotIndex);

            Debug.Log($"[SaveManager] 슬롯 {slotIndex} 새 통치 시작 (초기화 완료)");
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
