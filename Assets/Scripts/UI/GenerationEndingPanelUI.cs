using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wiggle.Data;
using Wiggle.Global;
using Wiggle.Systems;

namespace Wiggle.UI
{
    public class GenerationEndingPanelUI : MonoBehaviour
    {
        [Header("Root")]
        public GameObject panelRoot;

        [Header("Texts")]
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI summaryText;
        public TextMeshProUGUI rewardText;
        public TextMeshProUGUI continueButtonText;

        [Header("Controls")]
        public Button continueButton;

        private GameStatus status;
        private CanvasGroup panelCanvasGroup;

        private void Awake()
        {
            status = DataHub.Status;
            if (panelRoot == null)
                panelRoot = gameObject;

            panelCanvasGroup = panelRoot.GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            status = status != null ? status : DataHub.Status;

            if (status != null)
                status.OnGenerationEndingChanged += Refresh;

            if (continueButton != null)
            {
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(ContinueToNextGeneration);
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (status != null)
                status.OnGenerationEndingChanged -= Refresh;
        }

        private void Refresh()
        {
            SetPanelVisible(false);
        }

        private void ContinueToNextGeneration()
        {
            ReincarnationManager.Instance?.TriggerNormalReincarnation();
        }

        private void SetPanelVisible(bool visible)
        {
            if (panelRoot == null) return;

            if (panelRoot == gameObject)
            {
                if (panelCanvasGroup == null)
                    panelCanvasGroup = panelRoot.GetComponent<CanvasGroup>() ?? panelRoot.AddComponent<CanvasGroup>();

                panelCanvasGroup.alpha = visible ? 1f : 0f;
                panelCanvasGroup.interactable = visible;
                panelCanvasGroup.blocksRaycasts = visible;
                return;
            }

            panelRoot.SetActive(visible);
        }
    }
}
