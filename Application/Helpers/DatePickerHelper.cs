using Microsoft.AspNetCore.Http;

namespace Application.Helpers
{
    public static class DatePickerHelper
    {

        public static string? FixReversedDate(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            return new string(input.Reverse().ToArray());
        }
    }
}
