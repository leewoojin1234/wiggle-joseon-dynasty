using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wiggle.Global;
using Wiggle.Systems;

namespace Wiggle.UI
{
    public class ChoiceEventPanelUI : MonoBehaviour
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

        private ChoiceEventSystem eventSystem;

        private void Awake()
        {
            eventSystem = ChoiceEventSystem.Instance;
        }

        private void OnEnable()
        {
            eventSystem = eventSystem != null ? eventSystem : ChoiceEventSystem.Instance;

            if (eventSystem != null)
                eventSystem.OnPendingEventChanged += Refresh;

            if (optionAButton != null)
            {
                optionAButton.onClick.RemoveAllListeners();
                optionAButton.onClick.AddListener(() => eventSystem?.ChooseOptionA());
            }

            if (optionBButton != null)
            {
                optionBButton.onClick.RemoveAllListeners();
                optionBButton.onClick.AddListener(() => eventSystem?.ChooseOptionB());
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (eventSystem != null)
                eventSystem.OnPendingEventChanged -= Refresh;
        }

        private void Refresh()
        {
            if (eventSystem == null)
            {
                if (panelRoot != null) panelRoot.SetActive(false);
                return;
            }

            ChoiceEventDefinition pending = eventSystem.PendingEvent;
            bool hasEvent = pending != null;

            if (panelRoot != null)
                panelRoot.SetActive(hasEvent);

            if (!hasEvent) return;

            if (titleText != null) titleText.text = pending.title;
            if (descriptionText != null) descriptionText.text = pending.description;
            if (queuedCountText != null) queuedCountText.text = $"대기 {eventSystem.QueuedEventCount}";

            BindOption(pending.optionA, optionALabelText, optionAEffectText);
            BindOption(pending.optionB, optionBLabelText, optionBEffectText);
        }

        private void BindOption(ChoiceEventOption option, TextMeshProUGUI labelText, TextMeshProUGUI effectText)
        {
            if (option == null) return;

            if (labelText != null)
                labelText.text = option.label;

            if (effectText != null)
                effectText.text = FormatEffect(option);
        }

        private string FormatEffect(ChoiceEventOption option)
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
    }
}
