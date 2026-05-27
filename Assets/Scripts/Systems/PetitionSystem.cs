using System;
using UnityEngine;
using Wiggle.Data;
using Wiggle.Global;

namespace Wiggle.Systems
{
    public class PetitionSystem : MonoBehaviour
    {
        public static PetitionSystem Instance { get; private set; }

        [Header("Timing")]
        public float firstEventDelaySeconds = 45f;
        public float eventIntervalSeconds = 180f;

        [Header("Petitions")]
        [Tooltip("비워두면 Resources/Data/Petitions 아래의 PetitionEventData를 자동으로 읽습니다.")]
        public PetitionEventData[] petitionEvents;

        [Header("Runtime")]
        [SerializeField] private PetitionEventData pendingEvent;

        private float eventTimer;

        public PetitionEventData PendingEvent => pendingEvent;
        public bool HasPendingEvent => pendingEvent != null;

        public event Action OnPendingEventChanged;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            LoadPetitionsIfNeeded();
            eventTimer = firstEventDelaySeconds;
        }

        private void Update()
        {
            if (HasPendingEvent) return;

            eventTimer -= Time.deltaTime;
            if (eventTimer <= 0f)
            {
                ShowPetition();
                eventTimer = eventIntervalSeconds;
            }
        }

        public void GrantOfflinePetition(double offlineSeconds)
        {
            if (offlineSeconds < eventIntervalSeconds) return;

            if (!HasPendingEvent)
                ShowPetition();
        }

        public void ChooseOptionA() => ResolveOption(pendingEvent?.optionA);
        public void ChooseOptionB() => ResolveOption(pendingEvent?.optionB);

        public void ResolveOption(PetitionOption option)
        {
            if (pendingEvent == null || option == null) return;

            ApplyOption(option);

            pendingEvent = null;
            OnPendingEventChanged?.Invoke();
        }

        private void ShowPetition()
        {
            if (!HasPendingEvent)
                pendingEvent = PickRandomEvent();

            OnPendingEventChanged?.Invoke();
        }

        private PetitionEventData PickRandomEvent()
        {
            LoadPetitionsIfNeeded();

            if (petitionEvents == null || petitionEvents.Length == 0) return null;
            return petitionEvents[UnityEngine.Random.Range(0, petitionEvents.Length)];
        }

        private void ApplyOption(PetitionOption option)
        {
            GameStatus status = DataHub.Status;
            GameSettings settings = DataHub.Settings;
            if (status == null || settings == null) return;

            double incomePerSecond = EconomyFormula.GetNonWiggleIncomePerSecond(
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

        private void LoadPetitionsIfNeeded()
        {
            if (petitionEvents != null && petitionEvents.Length > 0) return;
            petitionEvents = Resources.LoadAll<PetitionEventData>("Data/Petitions");
        }
    }
}
