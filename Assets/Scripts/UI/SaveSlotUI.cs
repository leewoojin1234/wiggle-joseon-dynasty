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

        void Start()
        {
            RefreshUI();
            loadButton.onClick.AddListener(OnLoadClick);
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
            }
            else
            {
                slotNameText.text = $"빈 슬롯 {slotIndex + 1}";
                dateText.text = "-";
                infoText.text = "새로운 통치를 시작하십시오.";
                loadButton.GetComponentInChildren<TextMeshProUGUI>().text = "새로 시작";
            }
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
            
            // 시작 화면 UI 닫기
            if (gameObject.transform.parent != null)
                gameObject.transform.parent.gameObject.SetActive(false);
        }
    }
}
