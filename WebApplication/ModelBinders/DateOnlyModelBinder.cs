using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace WebApplication.ModelBinders
{
    public class DateOnlyModelBinder : IModelBinder
    {
        private static readonly string[] _formats = new[]
        {
            // without spaces (page flatpickr)
            "dd/MM/yyyy",
            "d/M/yyyy",
            "d/MM/yyyy",
            "dd/M/yyyy",
            // with spaces (layout flatpickr: "d / m / Y")
            "dd / MM / yyyy",
            "d / M / yyyy",
            "d / MM / yyyy",
            "dd / M / yyyy",
            // ISO
            "yyyy-MM-dd",
            "yyyy/MM/dd",
        };

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrWhiteSpace(value))
            {
                // Don't set a result - let the Required attribute handle it
                // This prevents the default 01/01/0001 from being set
                return Task.CompletedTask;
            }

            // Normalise "d / m / Y" (layout format with spaces) → "d/m/Y"
            var normalized = System.Text.RegularExpressions.Regex.Replace(value, @"\s*/\s*", "/").Trim();

            if (DateOnly.TryParseExact(normalized, _formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                bindingContext.Result = ModelBindingResult.Success(date);
            }
            else
            {
                // For invalid dates, don't set a result but add an error
                // This prevents the default 01/01/0001 from being displayed
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName,
                    "التاريخ غير صحيح. الرجاء إدخال تاريخ بصيغة يوم/شهر/سنة.");
            }

            return Task.CompletedTask;
        }
    }

    public class DateOnlyModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(DateOnly) ||
                context.Metadata.ModelType == typeof(DateOnly?))
            {
                return new DateOnlyModelBinder();
            }

            return null;
        }
    }
}
