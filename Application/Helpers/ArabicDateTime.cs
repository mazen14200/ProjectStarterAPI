using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public static class ArabicDateTime
    {
       public static string GetArabicDateTime(DateTime? date)
        {
            if (!date.HasValue) return string.Empty;

            var d = date.Value;
            var arabicNumbers = "٠١٢٣٤٥٦٧٨٩";

            // تنسيق التاريخ
            var year = d.Year.ToString("0000");
            var month = d.Month.ToString("00");
            var day = d.Day.ToString("00");

            // تنسيق الوقت
            var hour = d.ToString("hh");
            var minute = d.Minute.ToString("00");
            var second = d.Second.ToString("00");

            // تحديد الفترة (ص/م)
            var period = d.Hour < 12 ? "ص" : "م";

            // تحويل كل مكون على حدة
            var datePart = $"{ToArabicNumerals(year)}/{ToArabicNumerals(month)}/{ToArabicNumerals(day)}";
            var timePart = $"{ToArabicNumerals(hour)}:{ToArabicNumerals(minute)}:{ToArabicNumerals(second)} {period}";

            return $"{datePart} {timePart}";

            // دالة مساعدة داخلية لتحويل الأرقام
            string ToArabicNumerals(string input)
            {
                var result = input;
                for (int i = 0; i <= 9; i++)
                {
                    result = result.Replace(i.ToString()[0], arabicNumbers[i]);
                }
                return result;
            }
        }
    }

  
}
