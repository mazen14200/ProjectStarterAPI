namespace Domain.Consts
{
    /// <summary>
    /// مجموعة من الأنماط النمطية (Regex Patterns) المستخدمة في التحقق من صحة البيانات
    /// </summary>
    
    #region Example in VM
    ///   [RegularExpression(RegexPatterns.ArabicOrEnglishName, ErrorMessage = Errors.NameInvalidCharacters)]
    ///    public string FullName { get; set; } = string.Empty;
    #endregion Example in VM

    public static class RegexPatterns
    {
        #region Password Patterns

        /// <summary>
        /// كلمة مرور قوية: على الأقل 8 أحرف، تحتوي على حرف كبير وصغير ورقم ورمز خاص
        /// </summary>
        public const string StrongPassword = @"(?=(.*[0-9]))(?=.*[\!@#$%^&*()\\[\]{}\-_+=~`|:;""'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}";

        /// <summary>
        /// كلمة مرور متوسطة: على الأقل 6 أحرف، تحتوي على حرف ورقم
        /// </summary>
        public const string MediumPassword = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$";

        #endregion

        #region Phone Number Patterns

        /// <summary>
        /// رقم هاتف إماراتي: يبدأ بـ 05 أو +971 أو 00971
        /// </summary>
        public const string UAEPhone = @"^(?:\+971|00971|0)?5[024568]\d{7}$";

        /// <summary>
        /// رقم هاتف مصري: يبدأ بـ 01 أو +20 أو 0020
        /// </summary>
        public const string EgyptPhone = @"^(?:\+20|0020|0)?1[0125]\d{8}$";

        /// <summary>
        /// رقم هاتف سعودي
        /// </summary>
        public const string SaudiPhone = @"^(?:\+966|00966|0)?5[0-9]{8}$";

        #endregion

        #region Name Patterns

        /// <summary>
        /// اسم باللغة العربية فقط
        /// </summary>
        public const string ArabicName = @"^[\u0600-\u06FF\s]+$";

        /// <summary>
        /// اسم باللغة الإنجليزية فقط
        /// </summary>
        public const string EnglishName = @"^[a-zA-Z\s]+$";

        /// <summary>
        /// اسم بالعربية أو الإنجليزية
        /// </summary>
        public const string ArabicOrEnglishName = @"^[\u0600-\u06FFa-zA-Z\s]+$";

        #endregion

        #region Username Patterns

        /// <summary>
        /// اسم مستخدم: أحرف وأرقام فقط، من 3-50 حرف
        /// </summary>
        public const string SimpleUsername = @"^[a-zA-Z0-9]{3,50}$";

        /// <summary>
        /// اسم مستخدم متقدم: أحرف وأرقام وشرطة سفلية، يبدأ بحرف
        /// </summary>
        public const string Username = @"^[a-zA-Z][a-zA-Z0-9_]{2,19}$";

        #endregion

        #region Email Patterns

        /// <summary>
        /// بريد إلكتروني صحيح
        /// </summary>
        public const string Email = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        #endregion
    }
}
