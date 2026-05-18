using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Wiggle.Global;

namespace Wiggle.UI
{
    public class SaveSlotUI : MonoBehaviour
    {
        public int slotIndex;
        public TextMeshProUGUI slotNameText;
        public TextMeshProUGUI dateText;
        public TextMeshProUGUI infoText;
        public Button loadButton;
        public Button deleteButton; // 추가
        public StartScreenUI startScreen;

        void Start()
        {
            RefreshUI();
            loadButton.onClick.AddListener(OnLoadClick);
            if (deleteButton != null)
                deleteButton.onClick.AddListener(OnDeleteClick);
        }

        public void RefreshUI()
        {
            SaveData data = SaveManager.Instance.GetSaveInfo(slotIndex);
            if (data != null)
            {
                slotNameText.text = data.saveName;
                dateText.text = data.lastSaveDate;
                infoText.text = $"재산: {NumberFormatter.Format(data.money)} | 민심: {data.minSim:F0}";
                loadButton.GetComponentInChildren<TextMeshProUGUI>().text = "계속하기";
                if (deleteButton != null) deleteButton.gameObject.SetActive(true);
            }
            else
            {
                slotNameText.text = $"빈 슬롯 {slotIndex + 1}";
                dateText.text = "-";
                infoText.text = "새로운 통치를 시작하십시오.";
                loadButton.GetComponentInChildren<TextMeshProUGUI>().text = "새로 시작";
                if (deleteButton != null) deleteButton.gameObject.SetActive(false);
            }
        }

        private void OnDeleteClick()
        {
            DeleteConfirmPopupUI.Instance.Show(
                $"{slotIndex + 1}번 슬롯의 기록을 소멸시키겠습니까?\n(다시는 되돌릴 수 없습니다!)",
                () => {
                    SaveManager.Instance.DeleteSave(slotIndex);
                    RefreshUI();
                }
            );
        }


        private void OnLoadClick()
        {
            if (SaveManager.Instance.LoadGame(slotIndex))
            {
                Debug.Log("기존 데이터를 불러왔습니다.");
            }
            else
            {
                Debug.Log("새 게임을 시작합니다.");
                SaveManager.Instance.NewGame(slotIndex);
            }
            
            CloseStartScreen();
        }

        private void CloseStartScreen()
        {
            StartScreenUI screen = startScreen;

            if (screen == null)
                screen = GetComponentInParent<StartScreenUI>();

            if (screen == null)
                screen = StartScreenUI.Instance;

            if (screen != null)
            {
                screen.EnterGame();
                return;
            }

            // 기존 씬 호환용 fallback입니다. StartScreenUI를 붙이면 이 경로는 사용되지 않습니다.
            if (gameObject.transform.parent != null)
                gameObject.transform.parent.gameObject.SetActive(false);
        }
    }
}
