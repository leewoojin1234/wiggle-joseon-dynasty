using UnityEngine;

namespace Wiggle.Global
{
    public static class NumberFormatter
    {
        // 한국식 4자리 단위 (만, 억, 조, 경, 해, 자, 양, 구, 간, 정, 재, 극)
        private static readonly string[] units = new string[] 
        { 
            "", "만", "억", "조", "경", "해", "자", "양", "구", "간", "정", "재", "극" 
        };

        public static string Format(double value)
        {
            // 1만 미만은 그대로 표기 (소수점 버림)
            if (value < 10000d)
            {
                return value.ToString("0");
            }

            int unitIndex = 0;
            double formattedValue = value;

            // 값이 10,000 이상이고, 준비된 단위 배열을 넘지 않을 때까지 10,000으로 나눔
            while (formattedValue >= 10000d && unitIndex < units.Length - 1)
            {
                formattedValue /= 10000d;
                unitIndex++;
            }

            // 소수점 둘째 자리까지 표기 (예: 1.23만, 45.67억)
            // 한국식은 보통 소수점보다는 정수 비중이 높으나, 가독성을 위해 2자리 유지
            return formattedValue.ToString("0.##") + units[unitIndex];
        }
    }
}
