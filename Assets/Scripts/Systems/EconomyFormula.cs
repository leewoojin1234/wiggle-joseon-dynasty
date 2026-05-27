using UnityEngine;
using Wiggle.Data;
using Wiggle.Global;

namespace Wiggle.Systems
{
    public static class EconomyFormula
    {
        public static float GetSentimentModifier(float minSim)
        {
            if (minSim >= 50f)
                return Mathf.Lerp(1.0f, 1.5f, (minSim - 50f) / 50f);

            return Mathf.Lerp(0.2f, 1.0f, minSim / 50f);
        }

        public static double GetPassiveBaseIncome(GameSettings settings, HelperSystem helperSystem, InvestmentSystem investmentSystem)
        {
            if (settings == null) return 0;

            double helperIncome = helperSystem != null ? helperSystem.GetCurrentTotalHelperIncome() : 0;
            double investmentIncome = investmentSystem != null ? investmentSystem.GetCurrentTotalPassiveIncome() : 0;

            return settings.baseIncomePerSecond + helperIncome + investmentIncome;
        }

        public static double ApplyCoreMultipliers(double baseProduction, GameStatus status)
        {
            if (status == null) return 0;

            double income = baseProduction * status.wigglePower * GetSentimentModifier(status.minSim);

            if (DataHub.PermanentStatus != null)
                income *= DataHub.PermanentStatus.GetIncomeMultiplier();

            return income;
        }

        public static double ApplyRealmMultipliersWithoutWiggle(double baseProduction, GameStatus status)
        {
            if (status == null) return 0;

            double income = baseProduction * GetSentimentModifier(status.minSim);

            if (DataHub.PermanentStatus != null)
                income *= DataHub.PermanentStatus.GetIncomeMultiplier();

            return income;
        }

        public static double GetIncomePerSecond(GameStatus status, GameSettings settings, HelperSystem helperSystem, InvestmentSystem investmentSystem)
        {
            double baseProduction = GetPassiveBaseIncome(settings, helperSystem, investmentSystem);
            return ApplyCoreMultipliers(baseProduction, status);
        }

        public static double GetNonWiggleIncomePerSecond(GameStatus status, GameSettings settings, HelperSystem helperSystem, InvestmentSystem investmentSystem)
        {
            double baseProduction = GetPassiveBaseIncome(settings, helperSystem, investmentSystem);
            return ApplyRealmMultipliersWithoutWiggle(baseProduction, status);
        }

        public static double GetTaxBaseIncome(GameStatus status, GameSettings settings, HelperSystem helperSystem, InvestmentSystem investmentSystem)
        {
            return GetNonWiggleIncomePerSecond(status, settings, helperSystem, investmentSystem);
        }

        public static double GetClickIncome(GameStatus status, GameSettings settings)
        {
            if (settings == null) return 0;
            return ApplyCoreMultipliers(settings.clickBaseIncome, status);
        }
    }
}
