using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
    // SECURITY FIX: Input validation model example
    // This demonstrates proper input validation to prevent injection attacks
    // Never trust user input - always validate on the server side
    public class UserInputModel
    {
        // SECURITY FIX: Email validation with regex pattern
        // Prevents invalid email formats and potential injection attempts
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", 
            ErrorMessage = "Email format is invalid")]
        public string Email { get; set; } = string.Empty;

        // SECURITY FIX: Password validation with complexity requirements
        // Enforces strong password policy to prevent brute force attacks
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 12, 
            ErrorMessage = "Password must be between 12 and 100 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+])[A-Za-z\d!@#$%^&*()_+]{12,}$",
            ErrorMessage = "Password must contain uppercase, lowercase, number, and special character")]
        public string Password { get; set; } = string.Empty;

        // SECURITY FIX: Name validation to prevent XSS
        // Allows only letters, spaces, and common name characters
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 2, 
            ErrorMessage = "Name must be between 2 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z\s\-']+$", 
            ErrorMessage = "Name can only contain letters, spaces, hyphens, and apostrophes")]
        public string Name { get; set; } = string.Empty;

        // SECURITY FIX: Phone number validation
        // Prevents injection through phone input fields
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [RegularExpression(@"^\+?[\d\s\-()]{10,20}$", 
            ErrorMessage = "Phone number format is invalid")]
        public string Phone { get; set; } = string.Empty;

        // SECURITY FIX: Username validation
        // Prevents SQL injection and XSS through username
        [Required(ErrorMessage = "Username is required")]
        [StringLength(30, MinimumLength = 3, 
            ErrorMessage = "Username must be between 3 and 30 characters")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", 
            ErrorMessage = "Username can only contain letters, numbers, and underscores")]
        public string Username { get; set; } = string.Empty;

        // SECURITY FIX: Website URL validation
        // Prevents malicious URL injection
        [Url(ErrorMessage = "Invalid URL format")]
        [StringLength(200, ErrorMessage = "URL cannot exceed 200 characters")]
        [RegularExpression(@"^https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)$",
            ErrorMessage = "URL must start with http:// or https://")]
        public string? Website { get; set; }

        // SECURITY FIX: Age validation with range
        // Prevents invalid age values
        [Range(18, 120, ErrorMessage = "Age must be between 18 and 120")]
        public int Age { get; set; }

        // SECURITY FIX: Comment validation to prevent XSS
        // Sanitizes and limits user comments
        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,!?@#$%&()\-_]+$", 
            ErrorMessage = "Comment contains invalid characters")]
        public string? Comment { get; set; }
    }

    // SECURITY FIX: Example of a secure form model with CSRF protection
    // Use this model in forms that modify data
    public class SecureFormModel
    {
        // SECURITY FIX: Always include anti-forgery token in forms
        // The token is automatically validated with [ValidateAntiForgeryToken] attribute
        public string? AntiForgeryToken { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [StringLength(100, ErrorMessage = "Field cannot exceed 100 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-_]+$", 
            ErrorMessage = "Field contains invalid characters")]
        public string SecureField { get; set; } = string.Empty;
    }

    // SECURITY FIX: Query parameter validation model
    // Use this to validate query string parameters
    public class QueryParameterModel
    {
        // SECURITY FIX: Validate ID parameters to prevent SQL injection
        [Required(ErrorMessage = "ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ID must be a positive number")]
        public int Id { get; set; }

        // SECURITY FIX: Validate search terms to prevent injection
        [StringLength(100, ErrorMessage = "Search term cannot exceed 100 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-_]+$", 
            ErrorMessage = "Search term contains invalid characters")]
        public string? SearchTerm { get; set; }

        // SECURITY FIX: Validate pagination parameters
        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 10;

        [Range(1, int.MaxValue, ErrorMessage = "Page number must be positive")]
        public int PageNumber { get; set; } = 1;
    }
}
