using System.ComponentModel.DataAnnotations;

namespace WebApplicationAPI.Models.AuthAndUser;

public class TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTimeOffset AccessTokenExpiresAt { get; set; }
    public string TokenType { get; init; } = "Bearer";
}

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
