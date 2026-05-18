using System;
using UnityEngine;
using Wiggle.Data;
using Wiggle.Global;

namespace Wiggle.Systems
{
    public class EconomySystem : MonoBehaviour
    {
        public static EconomySystem Instance { get; private set; }

        private GameStatus status;
        private GameSettings settings;
        public HelperSystem helperSystem;
        public InvestmentSystem investmentSystem;

        public double CurrentIncomePerSecond { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            status ??= DataHub.Status;
            settings ??= DataHub.Settings;
        }

        void Update()
        {
            if (status == null || settings == null) return;

            var hSystem = helperSystem != null ? helperSystem : HelperSystem.Instance;
            var iSystem = investmentSystem != null ? investmentSystem : InvestmentSystem.Instance;
            CurrentIncomePerSecond = EconomyFormula.GetIncomePerSecond(status, settings, hSystem, iSystem);
            status.AddMoney(CurrentIncomePerSecond * Time.deltaTime);
        }

        public double CollectRoyalWiggle()
        {
            if (status == null || settings == null) return 0;

            double amount = EconomyFormula.GetClickIncome(status, settings);
            status.AddMoney(amount);
            return amount;
        }

        public double CalculateTaxReward(float multiplier)
        {
            if (status == null || settings == null) return 0;

            var hSystem = helperSystem != null ? helperSystem : HelperSystem.Instance;
            var iSystem = investmentSystem != null ? investmentSystem : InvestmentSystem.Instance;
            return EconomyFormula.GetIncomePerSecond(status, settings, hSystem, iSystem) * multiplier;
        }
    }
}
