using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationAPI.Services
{

    #region Example in VM
        //  [Display(Name = "ملف مرفق")]
        //  [FileValidation(
        //  MaxFileSizeInBytes = 5 * 1024 * 1024,
        //  AllowedExtensions = new[] { ".pdf" },
        //  ErrorMessage = "الملف يجب أن يكون PDF وألا يزيد عن 5 ميجابايت"
        //  )]
        //  public IFormFile? AttachmentFile { get; set; }
    #endregion Example in VM

    public class FileValidationAttribute : ValidationAttribute, IClientModelValidator
    {
        public long MaxFileSizeInBytes { get; set; } = 10 * 1024 * 1024; // 10MB افتراضي
        public string[] AllowedExtensions { get; set; } = new[] { ".pdf" };

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file == null)
                return ValidationResult.Success;

            if (!AllowedExtensions.Contains(System.IO.Path.GetExtension(file.FileName).ToLower()))
            {
                return new ValidationResult(ErrorMessage ?? $"يجب أن يكون الملف من نوع: {string.Join(", ", AllowedExtensions)}");
            }

            if (file.Length > MaxFileSizeInBytes)
            {
                return new ValidationResult(ErrorMessage ?? $"حجم الملف لا يجب أن يزيد عن {MaxFileSizeInBytes / (1024 * 1024)} ميجابايت");
            }

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // إضافة السمات data-val
            context.Attributes["data-val"] = "true";
            context.Attributes["data-val-filevalidation"] = ErrorMessage ?? "خطأ في التحقق من صحة الملف";

            // إضافة معلمات للتحقق
            context.Attributes["data-val-filevalidation-maxsize"] = MaxFileSizeInBytes.ToString();
            context.Attributes["data-val-filevalidation-extensions"] = string.Join(",", AllowedExtensions);

            // إضافة message مخصص
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                context.Attributes["data-val-filevalidation"] = ErrorMessage;
            }
        }

    }
}