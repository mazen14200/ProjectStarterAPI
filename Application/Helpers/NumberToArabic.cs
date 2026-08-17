using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public static class NumberToArabic
    {
        public static string NumberToArabicOrdinalFeminine(int? number)
        {
            if (number == null)
                return string.Empty;
            if (number <= 0 || number > 99)
                return string.Empty;

            string[] ones =
            {
        "", "الأولى", "الثانية", "الثالثة", "الرابعة",
        "الخامسة", "السادسة", "السابعة", "الثامنة", "التاسعة"
    };

            string[] ordinals11to19 =
            {
        "العاشرة",
        "الحادية عشرة",
        "الثانية عشرة",
        "الثالثة عشرة",
        "الرابعة عشرة",
        "الخامسة عشرة",
        "السادسة عشرة",
        "السابعة عشرة",
        "الثامنة عشرة",
        "التاسعة عشرة"
    };

            string[] tens =
            {
        "", "", "العشرون", "الثلاثون", "الأربعون",
        "الخمسون", "الستون", "السبعون", "الثمانون", "التسعون"
    };

            if (number < 10)
                return ones[number.Value];

            if (number >= 10 && number.Value <= 19)
                return ordinals11to19[number.Value - 10];

            int t = number.Value / 10;
            int o = number.Value % 10;

            if (o == 0)
                return tens[t];

            return $"{ones[o].Replace("الأ", "ال")} و {tens[t]}";
        }
    }
}
