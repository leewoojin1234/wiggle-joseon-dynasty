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

        /// <summary>
        /// 모든 게임 상태를 완전히 초기값으로 되돌립니다.
        /// </summary>
        public static void ResetAllData()
        {
            if (Status != null) Status.Initialize(Settings != null ? Settings.wiggleBase : 1.0f);
            if (HelperStatus != null) HelperStatus.ResetData();
            if (InvestmentStatus != null) InvestmentStatus.ResetData();
            
            Debug.Log("[DataHub] 모든 데이터가 성공적으로 초기화되었습니다.");
        }
    }
}
