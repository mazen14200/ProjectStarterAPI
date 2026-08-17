using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace WebApplication.ModelBinders
{
    public class TimeOnlyModelBinder : IModelBinder
    {
        // Formats tried against the NORMALISED value (Arabic replaced with English)
        private static readonly string[] _formats = new[]
        {
            // 24-hour formats (what the page flatpickr sends)
            "HH:mm",
            "H:mm",
            "HH:mm:ss",
            // 12-hour English AM/PM (what asp-format "{0:hh:mm tt}" renders,
            // and what the layout flatpickr sends after our Arabic→English replacement)
            "hh:mm tt",
            "h:mm tt",
            "hh:mm",
            "h:mm",
        };

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult == ValueProviderResult.None)
                return Task.CompletedTask;

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrWhiteSpace(value))
            {
                // Don't set a result - let the Required attribute handle it
                // This prevents the default 00:00:00 from being set
                return Task.CompletedTask;
            }

            // Replace Arabic AM/PM markers with English equivalents
            var normalized = value
                .Replace("ص", "AM")
                .Replace("م", "PM")
                .Trim();

            // Try all explicit formats first
            if (TimeOnly.TryParseExact(normalized, _formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
            {
                bindingContext.Result = ModelBindingResult.Success(time);
                return Task.CompletedTask;
            }

            // Fallback: let the runtime try any culture it likes
            if (TimeOnly.TryParse(normalized, CultureInfo.InvariantCulture, DateTimeStyles.None, out var time2))
            {
                bindingContext.Result = ModelBindingResult.Success(time2);
                return Task.CompletedTask;
            }

            // For invalid times, don't set a result but add an error
            // This prevents the default 00:00:00 from being displayed
            bindingContext.ModelState.TryAddModelError(
                bindingContext.ModelName,
                "الوقت غير صحيح. الرجاء إدخال وقت صحيح.");

            return Task.CompletedTask;
        }
    }

    public class TimeOnlyModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(TimeOnly) ||
                context.Metadata.ModelType == typeof(TimeOnly?))
            {
                return new TimeOnlyModelBinder();
            }
            return null;
        }
    }
}
