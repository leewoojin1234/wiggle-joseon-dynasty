using UnityEngine;

namespace Wiggle.Global
{
    public static class NumberFormatter
    {
        // K, M, B, T 및 그 이후 단위들
        private static readonly string[] units = new string[] 
        { 
            "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No", "Dc", "Ud", "Dd", "Td" 
        };

        public static string Format(double value)
        {
            // 1000 미만은 그대로 표기 (소수점 버림)
            if (value < 1000d)
            {
                return value.ToString("0");
            }

            int unitIndex = 0;
            double formattedValue = value;

            // 값이 1000 이상이고, 준비된 단위 배열을 넘지 않을 때까지 1000으로 나눔
            while (formattedValue >= 1000d && unitIndex < units.Length - 1)
            {
                formattedValue /= 1000d;
                unitIndex++;
            }

            // 소수점 둘째 자리까지 표기 (예: 1.23K, 45.6M)
            return formattedValue.ToString("0.##") + units[unitIndex];
        }
    }
}
