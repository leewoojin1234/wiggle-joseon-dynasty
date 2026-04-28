using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Wiggle.UI
{
    public class DeleteConfirmPopupUI : MonoBehaviour
    {
        public static DeleteConfirmPopupUI Instance { get; private set; }

        public TextMeshProUGUI messageText;
        public Button confirmButton;
        public Button cancelButton;

        private Action _onConfirm;

        void Awake()
        {
            Instance = this;
            gameObject.SetActive(false); // 처음엔 숨김
            
            cancelButton.onClick.AddListener(() => gameObject.SetActive(false));
            confirmButton.onClick.AddListener(() => {
                _onConfirm?.Invoke();
                gameObject.SetActive(false);
            });
        }

        public void Show(string message, Action onConfirm)
        {
            messageText.text = message;
            _onConfirm = onConfirm;
            gameObject.SetActive(true);
        }
    }
}
