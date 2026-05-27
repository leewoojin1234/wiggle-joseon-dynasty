using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wiggle.Data;
using Wiggle.Systems;

namespace Wiggle.UI
{
    public class PetitionPanelUI : MonoBehaviour
    {
        [Header("Root")]
        public GameObject panelRoot;

        [Header("Texts")]
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI queuedCountText;
        public TextMeshProUGUI optionALabelText;
        public TextMeshProUGUI optionBLabelText;
        public TextMeshProUGUI optionAEffectText;
        public TextMeshProUGUI optionBEffectText;

        [Header("Buttons")]
        public Button optionAButton;
        public Button optionBButton;

        private PetitionSystem petitionSystem;
        private CanvasGroup panelCanvasGroup;

        private void Awake()
        {
            petitionSystem = PetitionSystem.Instance;
            if (panelRoot != null)
                panelCanvasGroup = panelRoot.GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            petitionSystem = petitionSystem != null ? petitionSystem : PetitionSystem.Instance;

            if (petitionSystem != null)
                petitionSystem.OnPendingEventChanged += Refresh;

            if (optionAButton != null)
            {
                optionAButton.onClick.RemoveAllListeners();
                optionAButton.onClick.AddListener(() => petitionSystem?.ChooseOptionA());
            }

            if (optionBButton != null)
            {
                optionBButton.onClick.RemoveAllListeners();
                optionBButton.onClick.AddListener(() => petitionSystem?.ChooseOptionB());
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (petitionSystem != null)
                petitionSystem.OnPendingEventChanged -= Refresh;
        }

        private void Refresh()
        {
            if (petitionSystem == null)
            {
                SetPanelVisible(false);
                return;
            }

            PetitionEventData pending = petitionSystem.PendingEvent;
            bool hasEvent = pending != null;

            SetPanelVisible(hasEvent);

            if (!hasEvent) return;

            if (titleText != null) titleText.text = pending.title;
            if (descriptionText != null) descriptionText.text = pending.description;
            if (queuedCountText != null) queuedCountText.text = "상소 도착";

            BindOption(pending.optionA, optionALabelText, optionAEffectText);
            BindOption(pending.optionB, optionBLabelText, optionBEffectText);
        }

        private void BindOption(PetitionOption option, TextMeshProUGUI labelText, TextMeshProUGUI effectText)
        {
            if (option == null) return;

            if (labelText != null)
                labelText.text = option.label;

            if (effectText != null)
                effectText.text = FormatEffect(option);
        }

        private string FormatEffect(PetitionOption option)
        {
            string money = option.incomeSeconds >= 0
                ? $"+{option.incomeSeconds:0}초 수익"
                : $"{option.incomeSeconds:0}초 수익";

            string minSim = Mathf.Approximately(option.minSimDelta, 0f)
                ? "민심 0"
                : $"민심 {(option.minSimDelta > 0 ? "+" : "")}{option.minSimDelta:0}";

            string wiggle = Mathf.Approximately(option.wiggleDelta, 0f)
                ? string.Empty
                : $" / 실룩 {(option.wiggleDelta > 0 ? "+" : "")}{option.wiggleDelta:0.0}";

            return $"{money} / {minSim}{wiggle}";
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
