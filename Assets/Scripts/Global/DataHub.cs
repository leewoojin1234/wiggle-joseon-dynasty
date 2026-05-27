using UnityEngine;
using Wiggle.Data;

namespace Wiggle.Global
{
    /// <summary>
    /// 조선의 중앙 집권 체제처럼, 모든 데이터를 한곳에서 관리하고 제공합니다.
    /// Resources/Data 폴더에 있는 에셋을 자동으로 로드하며, 싱글톤처럼 유일성을 보장합니다.
    /// </summary>
    public static class DataHub
    {
        private static GameStatus _status;
        public static GameStatus Status
        {
            get
            {
                if (_status == null) _status = Resources.Load<GameStatus>("Data/GameStatus");
                if (_status == null) Debug.LogError("[DataHub] Resources/Data/GameStatus 에셋이 없습니다!");
                return _status;
            }
        }

        private static GameSettings _settings;
        public static GameSettings Settings
        {
            get
            {
                if (_settings == null) _settings = Resources.Load<GameSettings>("Data/GameSettings");
                if (_settings == null) Debug.LogError("[DataHub] Resources/Data/GameSettings 에셋이 없습니다!");
                return _settings;
            }
        }

        private static HelperStatus _helperStatus;
        public static HelperStatus HelperStatus
        {
            get
            {
                if (_helperStatus == null) _helperStatus = Resources.Load<HelperStatus>("Data/HelperStatus");
                if (_helperStatus == null) Debug.LogError("[DataHub] Resources/Data/HelperStatus 에셋이 없습니다!");
                return _helperStatus;
            }
        }

        private static HelperStatus _investmentStatus;
        public static HelperStatus InvestmentStatus
        {
            get
            {
                if (_investmentStatus == null) _investmentStatus = Resources.Load<HelperStatus>("Data/InvestmentStatus");
                if (_investmentStatus == null) Debug.LogError("[DataHub] Resources/Data/InvestmentStatus 에셋이 없습니다!");
                return _investmentStatus;
            }
        }

        private static PermanentStatus _permanentStatus;
        public static PermanentStatus PermanentStatus
        {
            get
            {
                if (_permanentStatus == null) _permanentStatus = Resources.Load<PermanentStatus>("Data/PermanentStatus");
                if (_permanentStatus == null) Debug.LogError("[DataHub] Resources/Data/PermanentStatus 에셋이 없습니다!");
                return _permanentStatus;
            }
        }

        /// <summary>
        /// 모든 게임 상태를 완전히 초기값으로 되돌립니다. (환생 시의 초기화와 다름)
        /// </summary>
        public static void ResetAllData()
        {
            if (Status != null)
            {
                float baseWiggle = Settings != null ? Settings.wiggleBase : 1.0f;
                float lifeSpanSeconds = Settings != null ? Settings.rulerLifeSpanSeconds : 600f;
                Status.ResetToDefault(baseWiggle, lifeSpanSeconds);
            }
            if (HelperStatus != null) HelperStatus.ResetData();
            if (InvestmentStatus != null) InvestmentStatus.ResetData();
            // PermanentStatus는 지우지 않습니다 (사용자가 '데이터 완전 삭제'를 원할 때만 별도 호출)

            Debug.Log("[DataHub] 모든 실시간 데이터가 초기화되었습니다.");
        }

        /// <summary>
        /// 영구 업그레이드 데이터를 포함한 모든 데이터를 삭제합니다.
        /// </summary>
        public static void WipeEverything()
        {
            ResetAllData();
            if (PermanentStatus != null) PermanentStatus.ResetAll();
            Debug.Log("[DataHub] 영구 데이터 포함 모든 기록이 소멸되었습니다.");
        }

    }
}
