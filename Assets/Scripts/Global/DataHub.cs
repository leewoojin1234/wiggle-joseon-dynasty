using UnityEngine;
using Wiggle.Data;

namespace Wiggle.Global
{
    /// <summary>
    /// 조선의 중앙 집권 체제처럼, 모든 데이터를 한곳에서 관리하고 제공합니다.
    /// Resources/Data 폴더에 있는 에셋을 자동으로 로드합니다.
    /// </summary>
    public static class DataHub
    {
        private static GameStatus _status;
        public static GameStatus Status
        {
            get
            {
                if (_status == null)
                {
                    _status = Resources.Load<GameStatus>("Data/GameStatus");
                    if (_status == null) Debug.LogError("Resources/Data/GameStatus 에셋을 찾을 수 없습니다!");
                }
                return _status;
            }
        }

        private static GameSettings _settings;
        public static GameSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = Resources.Load<GameSettings>("Data/GameSettings");
                    if (_settings == null) Debug.LogError("Resources/Data/GameSettings 에셋을 찾을 수 없습니다!");
                }
                return _settings;
            }
        }
    }
}
