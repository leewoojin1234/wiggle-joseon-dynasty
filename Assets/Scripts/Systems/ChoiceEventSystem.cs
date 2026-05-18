using System;
using UnityEngine;
using Wiggle.Data;
using Wiggle.Global;

namespace Wiggle.Systems
{
    [Serializable]
    public class ChoiceEventOption
    {
        public string label;
        public double incomeSeconds;
        public float minSimDelta;
        public float wiggleDelta;
    }

    [Serializable]
    public class ChoiceEventDefinition
    {
        public string id;
        public string title;
        [TextArea] public string description;
        public ChoiceEventOption optionA;
        public ChoiceEventOption optionB;
    }

    public class ChoiceEventSystem : MonoBehaviour
    {
        public static ChoiceEventSystem Instance { get; private set; }

        [Header("Timing")]
        public float firstEventDelaySeconds = 45f;
        public float eventIntervalSeconds = 180f;
        public int maxQueuedEvents = 3;

        [Header("Runtime")]
        [SerializeField] private ChoiceEventDefinition pendingEvent;
        [SerializeField] private int queuedEventCount;

        private float eventTimer;

        public ChoiceEventDefinition PendingEvent => pendingEvent;
        public int QueuedEventCount => queuedEventCount;
        public bool HasPendingEvent => pendingEvent != null;

        public event Action OnPendingEventChanged;

        private ChoiceEventDefinition[] defaultEvents;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            defaultEvents = CreateDefaultEvents();
            eventTimer = firstEventDelaySeconds;
        }

        private void Update()
        {
            if (HasPendingEvent) return;

            eventTimer -= Time.deltaTime;
            if (eventTimer <= 0f)
            {
                QueueEvent();
                eventTimer = eventIntervalSeconds;
            }
        }

        public void GrantOfflineChoices(double offlineSeconds)
        {
            if (offlineSeconds < eventIntervalSeconds) return;

            int earned = Mathf.Clamp((int)(offlineSeconds / eventIntervalSeconds), 1, maxQueuedEvents);
            queuedEventCount = Mathf.Clamp(queuedEventCount + earned, 0, maxQueuedEvents);

            if (!HasPendingEvent)
                QueueEvent();
        }

        public void ChooseOptionA() => ResolveOption(pendingEvent?.optionA);
        public void ChooseOptionB() => ResolveOption(pendingEvent?.optionB);

        public void ResolveOption(ChoiceEventOption option)
        {
            if (pendingEvent == null || option == null) return;

            ApplyOption(option);

            pendingEvent = null;
            queuedEventCount = Mathf.Max(0, queuedEventCount - 1);

            if (queuedEventCount > 0)
                pendingEvent = PickRandomEvent();

            OnPendingEventChanged?.Invoke();
        }

        private void QueueEvent()
        {
            queuedEventCount = Mathf.Clamp(queuedEventCount + 1, 0, maxQueuedEvents);
            if (!HasPendingEvent)
                pendingEvent = PickRandomEvent();

            OnPendingEventChanged?.Invoke();
        }

        private ChoiceEventDefinition PickRandomEvent()
        {
            if (defaultEvents == null || defaultEvents.Length == 0) return null;
            return defaultEvents[UnityEngine.Random.Range(0, defaultEvents.Length)];
        }

        private void ApplyOption(ChoiceEventOption option)
        {
            GameStatus status = DataHub.Status;
            GameSettings settings = DataHub.Settings;
            if (status == null || settings == null) return;

            double incomePerSecond = EconomyFormula.GetIncomePerSecond(
                status,
                settings,
                HelperSystem.Instance,
                InvestmentSystem.Instance);

            if (!Mathf.Approximately((float)option.incomeSeconds, 0f))
                status.AddMoney(incomePerSecond * option.incomeSeconds);

            if (!Mathf.Approximately(option.minSimDelta, 0f))
            {
                var sentimentSystem = FindObjectOfType<SentimentSystem>();
                if (sentimentSystem != null)
                    sentimentSystem.AddMinSim(option.minSimDelta);
                else
                    status.AddMinSim(option.minSimDelta);
            }

            if (!Mathf.Approximately(option.wiggleDelta, 0f))
                status.AddWiggle(option.wiggleDelta, settings.wiggleMax, settings.wiggleBase);
        }

        private static ChoiceEventDefinition[] CreateDefaultEvents()
        {
            return new[]
            {
                new ChoiceEventDefinition
                {
                    id = "bumper_harvest",
                    title = "풍년",
                    description = "각 고을에서 곡식이 넘쳐난다는 장계가 올라왔습니다.",
                    optionA = new ChoiceEventOption { label = "국고로 수납", incomeSeconds = 45, minSimDelta = -1 },
                    optionB = new ChoiceEventOption { label = "백성에게 풀기", incomeSeconds = 10, minSimDelta = 4 }
                },
                new ChoiceEventDefinition
                {
                    id = "people_petition",
                    title = "백성 청원",
                    description = "굶주린 백성들이 궁 앞에 모여 구휼을 청합니다.",
                    optionA = new ChoiceEventOption { label = "구휼 승인", incomeSeconds = -25, minSimDelta = 7 },
                    optionB = new ChoiceEventOption { label = "세금으로 돌려보내기", incomeSeconds = 35, minSimDelta = -6 }
                },
                new ChoiceEventDefinition
                {
                    id = "corrupt_official",
                    title = "탐관오리 보고",
                    description = "감찰관이 숨겨진 재물을 발견했다는 밀서를 올렸습니다.",
                    optionA = new ChoiceEventOption { label = "몰수해 환원", incomeSeconds = 25, minSimDelta = 2 },
                    optionB = new ChoiceEventOption { label = "국고만 채우기", incomeSeconds = 60, minSimDelta = -5 }
                }
            };
        }
    }
}
