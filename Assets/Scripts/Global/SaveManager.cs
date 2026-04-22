using UnityEngine;
using System.IO;
using Wiggle.Data;
using Wiggle.Systems;

namespace Wiggle.Global
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }
        public int CurrentSlotIndex { get; set; } = -1; // 현재 플레이 중인 슬롯 번호

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

        // 현재 게임 상태를 특정 슬롯에 저장
        public void SaveGame(int slotIndex)
        {
            CurrentSlotIndex = slotIndex; // 저장할 때 현재 슬롯으로 설정
            
            GameStatus status = DataHub.Status;
            HelperStatus helperStatus = HelperSystem.Instance?.helperStatus;
            HelperStatus investStatus = InvestmentSystem.Instance?.investmentStatus;

            SaveData data = new SaveData($"슬롯 {slotIndex + 1}");
            data.money = status.money;
            data.minSim = status.minSim;
            
            if (helperStatus != null) data.helperLevels = (int[])helperStatus.helperLevels.Clone();
            if (investStatus != null) data.investmentLevels = (int[])investStatus.helperLevels.Clone();

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath(slotIndex), json);
            Debug.Log($"슬롯 {slotIndex} 저장 완료: {SavePath(slotIndex)}");
        }

        // 특정 슬롯의 데이터를 불러와 게임에 적용
        public bool LoadGame(int slotIndex)
        {
            string path = SavePath(slotIndex);
            if (!File.Exists(path)) return false;

            CurrentSlotIndex = slotIndex; // 로드할 때 현재 슬롯으로 설정

            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // 데이터 적용
            GameStatus status = DataHub.Status;
            status.money = data.money;
            status.minSim = data.minSim;
            
            if (HelperSystem.Instance != null && data.helperLevels != null)
                HelperSystem.Instance.helperStatus.helperLevels = data.helperLevels;
                
            if (InvestmentSystem.Instance != null && data.investmentLevels != null)
                InvestmentSystem.Instance.investmentStatus.helperLevels = data.investmentLevels;

            status.NotifyMoneyChanged();
            status.AddMinSim(0); // 민심 UI 갱신 유도
            
            Debug.Log($"슬롯 {slotIndex} 로드 완료");
            return true;
        }

        public SaveData GetSaveInfo(int slotIndex)
        {
            string path = SavePath(slotIndex);
            if (!File.Exists(path)) return null;
            return JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        }

        // 완전히 새로운 게임 시작 (모든 상태 강제 초기화)
        public void NewGame(int slotIndex)
        {
            CurrentSlotIndex = slotIndex;

            // 1. 기본 수치 초기화
            DataHub.Status.Initialize(DataHub.Settings.wiggleBase);

            // 2. 조력자 상태 강제 초기화
            if (HelperSystem.Instance != null && HelperSystem.Instance.allHelpers != null)
            {
                HelperSystem.Instance.helperStatus.Initialize(HelperSystem.Instance.allHelpers.Length, true);
            }

            // 3. 투자 상태 강제 초기화
            if (InvestmentSystem.Instance != null && InvestmentSystem.Instance.allInvestments != null)
            {
                InvestmentSystem.Instance.investmentStatus.Initialize(InvestmentSystem.Instance.allInvestments.Length, true);
            }

            // 4. 즉시 파일로 저장하여 슬롯 생성
            SaveGame(slotIndex);

            Debug.Log($"새로운 통치가 시작되었습니다. (슬롯 {slotIndex} 생성됨)");
        }

        // --- 자동 저장 트리거 ---

        private void OnApplicationQuit()
        {
            AutoSaveInternal();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                AutoSaveInternal();
            }
        }

        private void AutoSaveInternal()
        {
            if (CurrentSlotIndex != -1)
            {
                SaveGame(CurrentSlotIndex);
                Debug.Log("게임 종료/일시정지로 인한 자동 저장이 완료되었습니다.");
            }
        }
    }
}
