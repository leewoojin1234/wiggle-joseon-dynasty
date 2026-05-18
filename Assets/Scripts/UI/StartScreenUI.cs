using UnityEngine;

namespace Wiggle.UI
{
    public class StartScreenUI : MonoBehaviour
    {
        public static StartScreenUI Instance { get; private set; }

        [Header("Roots")]
        [Tooltip("이어하기/새로 시작 후 꺼질 메인 화면 루트입니다. 비워두면 이 컴포넌트가 붙은 오브젝트를 끕니다.")]
        public GameObject startScreenRoot;

        [Tooltip("게임 시작 후 켤 게임 UI 루트입니다. 이미 켜져 있다면 비워둬도 됩니다.")]
        public GameObject gameplayRoot;

        private void Awake()
        {
            Instance = this;

            if (startScreenRoot == null)
                startScreenRoot = gameObject;
        }

        public void EnterGame()
        {
            if (gameplayRoot != null)
                gameplayRoot.SetActive(true);

            if (startScreenRoot != null)
                startScreenRoot.SetActive(false);
        }
    }
}
