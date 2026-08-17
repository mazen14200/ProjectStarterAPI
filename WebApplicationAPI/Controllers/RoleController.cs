using Application.DTOs.Auth;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class RoleController : ControllerBase
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }
    //[AllowAnonymous]
    [HttpPost("create-role")]
    public async Task<IActionResult> CreateRole([FromQuery] string roleName)
    {
        if (await _roleManager.RoleExistsAsync(roleName))
            return BadRequest("Role already exists");

        var result = await _roleManager.CreateAsync(new ApplicationRole(roleName));
        return result.Succeeded ? Ok("Role created") : BadRequest(result.Errors);
    }
    //[AllowAnonymous]
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user == null) return NotFound("User not found");

        var result = await _userManager.AddToRoleAsync(user, dto.RoleName);
        return result.Succeeded ? Ok("Role assigned") : BadRequest(result.Errors);
    }
    //[AllowAnonymous]
    [HttpPost("add-claim")]
    public async Task<IActionResult> AddClaim([FromBody] AddClaimDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user == null) return NotFound("User not found");

        var result = await _userManager.AddClaimAsync(user, new Claim(dto.ClaimType, dto.ClaimValue));
        return result.Succeeded ? Ok("Claim added") : BadRequest(result.Errors);
    }
    //[AllowAnonymous]
    [HttpGet("user-claims/{userId}")]
    public async Task<IActionResult> GetUserClaims(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound("User not found");

        var claims = await _userManager.GetClaimsAsync(user);
        return Ok(claims.Select(c => new { c.Type, c.Value }));
    }
    //[AllowAnonymous]
    [HttpPost("remove-claim")]
    public async Task<IActionResult> RemoveClaim([FromBody] AddClaimDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user == null) return NotFound("User not found");

        var result = await _userManager.RemoveClaimAsync(user, new Claim(dto.ClaimType, dto.ClaimValue));
        return result.Succeeded ? Ok("Claim removed") : BadRequest(result.Errors);
    }
}