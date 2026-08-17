using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Identity;
using WebApplicationAPI.Models.AuthAndUser;
using WebApplicationAPI.Services;

namespace WebApplicationAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenStore _refreshTokenStore;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IRefreshTokenStore refreshTokenStore,
        IConfiguration config,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
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
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not null)
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Unable to register with the provided details."
            });
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        
        if (!result.Succeeded)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Registration failed.",
                Detail = string.Join(", ", result.Errors.Select(e => e.Description))
            });
        }

        _logger.LogInformation("New user registered: {UserId}", user.Id);

        var tokens = await IssueTokenPair(user);
        return CreatedAtAction(nameof(Me), tokens);
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        var invalidCredentials = Unauthorized(new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Invalid email or password."
        });

        if (user is null)
        {
            return invalidCredentials;
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            _logger.LogWarning("Login attempt for locked-out account: {UserId}", user.Id);
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Account temporarily locked due to repeated failed attempts. Try again later."
            });
        }

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            await _userManager.AccessFailedAsync(user);
            return invalidCredentials;
        }

        await _userManager.ResetAccessFailedCountAsync(user);
        var tokens = await IssueTokenPair(user);
        return Ok(tokens);
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("auth")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
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

        _refreshTokenStore.Revoke(request.RefreshToken);

        var owner = await _userManager.FindByIdAsync(entry.UserId);
        if (owner is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Account no longer exists."
            });
        }

        var tokens = await IssueTokenPair(owner);
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

    private async Task<TokenResponse> IssueTokenPair(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var (accessToken, expiresAt) = _tokenService.GenerateAccessToken(user, roles);
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
