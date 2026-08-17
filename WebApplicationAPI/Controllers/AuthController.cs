using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WebApplicationAPI.Models.AuthAndUser;
using WebApplicationAPI.Services;

namespace WebApplicationAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserStore _userStore;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenStore _refreshTokenStore;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserStore userStore,
        ITokenService tokenService,
        IRefreshTokenStore refreshTokenStore,
        IConfiguration config,
        ILogger<AuthController> logger)
    {
        _userStore = userStore;
        _tokenService = tokenService;
        _refreshTokenStore = refreshTokenStore;
        _config = config;
        _logger = logger;
    }

    private TimeSpan RefreshTokenLifetime =>
        TimeSpan.FromDays(_config.GetValue("Jwt:RefreshTokenExpiryDays", 7));

    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    [AllowAnonymous]
    public IActionResult Register([FromBody] RegisterUserRequest request)
    {
        if (_userStore.FindByEmail(request.Email) is not null)
        {
            // Same generic message as a failed login would give — avoid
            // confirming/denying account existence to an anonymous caller.
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Unable to register with the provided details."
            });
        }

        var user = _userStore.Create(request.Email, request.Password, request.FullName);
        _logger.LogInformation("New user registered: {UserId}", user.Id);

        var tokens = IssueTokenPair(user);
        return CreatedAtAction(nameof(Me), tokens);
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _userStore.FindByEmail(request.Email);

        // Deliberately identical response whether the email doesn't exist or
        // the password is wrong — don't leak which one it was.
        var invalidCredentials = Unauthorized(new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Invalid email or password."
        });

        if (user is null)
        {
            return invalidCredentials;
        }

        if (_userStore.IsLockedOut(user))
        {
            _logger.LogWarning("Login attempt for locked-out account: {UserId}", user.Id);
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Account temporarily locked due to repeated failed attempts. Try again later."
            });
        }

        if (!_userStore.VerifyPassword(user, request.Password))
        {
            _userStore.RegisterFailedAttempt(user);
            return invalidCredentials;
        }

        _userStore.ResetFailedAttempts(user);
        var tokens = IssueTokenPair(user);
        return Ok(tokens);
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("auth")]
    [AllowAnonymous]
    public IActionResult Refresh([FromBody] RefreshTokenRequest request)
    {
        var entry = _refreshTokenStore.Validate(request.RefreshToken);
        if (entry is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Invalid or expired refresh token."
            });
        }

        // Refresh tokens are rotated: the old one is revoked and a new pair issued.
        _refreshTokenStore.Revoke(request.RefreshToken);

        var owner = _userStore.FindById(entry.UserId);
        if (owner is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Account no longer exists."
            });
        }

        var tokens = IssueTokenPair(owner);
        return Ok(tokens);
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout([FromBody] RefreshTokenRequest request)
    {
        _refreshTokenStore.Revoke(request.RefreshToken);
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public IActionResult Me()
    {
        return Ok(new
        {
            id = User.FindFirst("sub")?.Value,
            email = User.Identity?.Name,
            fullName = User.FindFirst("full_name")?.Value,
            roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value)
        });
    }

    private TokenResponse IssueTokenPair(User user)
    {
        var (accessToken, expiresAt) = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        _refreshTokenStore.Store(refreshToken, user.Id, RefreshTokenLifetime);

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = expiresAt
        };
    }
}
