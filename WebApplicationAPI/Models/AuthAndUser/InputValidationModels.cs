using System.ComponentModel.DataAnnotations;

namespace WebApplicationAPI.Models.AuthAndUser;

/// <summary>
/// Example registration/login DTOs showing the validation patterns to reuse
/// across the API. Apply the same [Required]/[RegularExpression]/[StringLength]
/// approach to any incoming request model — never trust client input.
/// </summary>
public class RegisterUserRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(256, ErrorMessage = "Email must not exceed 256 characters.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(128, MinimumLength = 12, ErrorMessage = "Password must be at least 12 characters.")]
    // At least one lowercase, one uppercase, one digit, one special character
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{12,}$",
        ErrorMessage = "Password must contain upper/lowercase letters, a digit, and a special character.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, MinimumLength = 2)]
    [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "Name contains invalid characters.")]
    public string FullName { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number.")]
    [StringLength(20)]
    public string? PhoneNumber { get; set; }
}

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

public class UsernameValidationModel
{
    [Required]
    [StringLength(32, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username may only contain letters, numbers, and underscores.")]
    public string Username { get; set; } = string.Empty;
}

public class UrlValidationModel
{
    [Required]
    [Url(ErrorMessage = "Invalid URL format.")]
    [StringLength(2048)]
    public string Url { get; set; } = string.Empty;
}

/// <summary>
/// Use for any endpoint accepting free-text query parameters (search, filter, etc.)
/// to cap length and reject control characters before they reach business logic.
/// </summary>
public class QueryParameterValidationModel
{
    [StringLength(200)]
    [RegularExpression(@"^[^<>\x00-\x1F]*$", ErrorMessage = "Query contains invalid characters.")]
    public string? Query { get; set; }

    [Range(1, 100)]
    public int PageSize { get; set; } = 20;

    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; } = 1;
}
