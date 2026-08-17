using System.ComponentModel.DataAnnotations;

namespace Domain.Consts
{
    /// <summary>
    /// رسائل الأخطاء المستخدمة في النظام
    /// Placeholders: {0} = Property Name, {1} = Max, {2} = Min
    /// </summary>
    #region Example in VM
    ///  [Required(ErrorMessage = Errors.Required)]
    //  [Display(Name = "الجهة")]
    //  public int? AuthorityId { get; set; }
    //  [Display(Name = "الفرع")]
    //  [StringLength(200, ErrorMessage = Errors.MaxLength)]
    #endregion Example in VM

    public static class Errors
    {
        #region General Errors

        public const string GenericError = "حدث خطأ غير متوقع. الرجاء المحاولة مرة أخرى لاحقاً.";
        public const string NotFoundError = "العنصر المطلوب غير موجود.";
        public const string UnauthorizedError = "ليس لديك الصلاحيات اللازمة للوصول إلى هذا المورد.";
        public const string ValidationError = "يرجى التحقق من البيانات المدخلة.";

        #endregion

        #region Common Validation Errors (Generic)

        /// <summary>
        /// {0} = اسم الحقل
        /// </summary>
        public const string Required = "{0} مطلوب";

        /// <summary>
        /// {0} = اسم الحقل، {1} = الحد الأقصى
        /// </summary>
        public const string MaxLength = "{0} يجب ألا يتجاوز {1} حرف";

        /// <summary>
        /// {0} = اسم الحقل، {1} = الحد الأدنى
        /// </summary>
        public const string MinLength = "{0} يجب ألا يقل عن {1} حرف";

        /// <summary>
        /// {0} = اسم الحقل، {1} = MaxLength، {2} = MinLength
        /// </summary>
        public const string StringLength = "{0} يجب أن يكون بين {2} و {1} حرف";

        /// <summary>
        /// {0} = اسم الحقل
        /// </summary>
        public const string InvalidFormat = "صيغة {0} غير صحيحة";

        /// <summary>
        /// {0} = اسم الحقل
        /// </summary>
        public const string AlreadyExists = "{0} موجود بالفعل";

        /// <summary>
        /// {0} = اسم الحقل الأول، {1} = اسم الحقل الثاني
        /// </summary>
        public const string MustMatch = "{0} يجب أن يطابق {1}";

        #endregion

        #region Specific Field Errors

        // Email
        public const string EmailInvalid = "صيغة البريد الإلكتروني غير صحيحة";

        // Phone
        public const string PhoneInvalid = "رقم الهاتف غير صحيح";
        public const string PhoneInvalidUAE = "رقم الهاتف الإماراتي غير صحيح (مثال: 05XXXXXXXX)";
        public const string PhoneInvalidEgypt = "رقم الهاتف المصري غير صحيح (مثال: 01XXXXXXXXX)";
        public const string PhoneInvalidSaudi = "رقم الهاتف السعودي غير صحيح (مثال: 05XXXXXXXX)";

        // Password
        public const string PasswordMismatch = "كلمة المرور غير متطابقة";
        public const string PasswordWeak = "كلمة المرور ضعيفة. يجب أن تحتوي على حرف كبير وصغير ورقم ورمز خاص";

        // Name
        public const string NameInvalidCharacters = "الاسم يجب أن يحتوي على أحرف عربية أو إنجليزية فقط";

        // Username
        public const string UsernameInvalidCharacters = "اسم المستخدم يجب أن يحتوي على أحرف وأرقام فقط";

        #endregion

        #region Authentication Errors

        public const string InvalidCredentials = "اسم المستخدم أو كلمة المرور غير صحيحة";
        public const string AccountLocked = "الحساب موقوف مؤقتاً. يرجى المحاولة لاحقاً";
        public const string AccountNotFound = "الحساب غير موجود";
        public const string EmailNotConfirmed = "يرجى تأكيد البريد الإلكتروني أولاً";

        #endregion

        #region File Upload Errors

        public const string FileRequired = "الملف مطلوب";
        public const string FileTooLarge = "حجم الملف كبير جداً. الحد الأقصى {0} ميجابايت";
        public const string InvalidFileType = "نوع الملف غير مدعوم";
        public const string ImageRequired = "الصورة مطلوبة";
        public const string InvalidImageFormat = "صيغة الصورة غير مدعومة (jpg, png, gif فقط)";

        #endregion
    }
}
