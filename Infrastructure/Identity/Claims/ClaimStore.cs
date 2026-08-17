using System.Security.Claims;

namespace Infrastructure.Identity.Claims
{
    public static class ClaimStore
    {
      
        public static List<Claim> RolesClaimsList = new List<Claim>
        {
            new Claim("ViewRoles", "عرض الأدوار"),
            new Claim("CreateRole", "إنشاء دور"),
            new Claim("EditRole", "تعديل دور"),
            new Claim("DeleteRole", "حذف دور"),
            new Claim("ManageRoleClaims", "إدارة صلاحيات الدور")
        };

        public static List<Claim> UsersClaimsList = new List<Claim>
        {
            new Claim("ViewUsers", "عرض المستخدمين"),
            new Claim("CreateUser", "إنشاء مستخدم"),
            new Claim("EditUser", "تعديل مستخدم"),
            new Claim("DeleteUser", "حذف مستخدم"),
            new Claim("ResetPassword", "إعادة تعيين كلمة المرور")
        };

        public static List<Claim> MessagesClaimsList = new List<Claim>
        {
            new Claim("ViewMessages", "عرض الرسائل"),
            new Claim("SendMessages", "إرسال الرسائل")
        };

        public static List<Claim> WeatherForecast = new List<Claim>
        {
            new Claim("GetWeatherForecast", "عرض الطقس"),
        };
    }
}