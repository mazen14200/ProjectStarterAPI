using Application.DTOs;
using Application.Interfaces.Services;
using Infrastructure.DbContext;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace Infrastructure.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly AppDbContext _context;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<UserDto?> GetByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return null;

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        public async Task<bool> CreateAsync(CreateUserDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            return result.Succeeded;
        }

        public async Task<bool> AssignRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return false;

            if (!await _roleManager.RoleExistsAsync(role))
                return false;

            var result = await _userManager.AddToRoleAsync(user, role);

            return result.Succeeded;
        }

        public async Task<string?> GenerateRefreshTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            // Generate a cryptographically secure random token
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshToken = Convert.ToBase64String(randomNumber);

            // Store the refresh token (you would typically have a RefreshToken entity)
            // For now, we'll use the user's SecurityStamp as a simple storage mechanism
            // In production, create a proper RefreshToken entity with expiry dates
            var result = await _userManager.SetAuthenticationTokenAsync(user, "RefreshToken", "CurrentToken", refreshToken);
            
            if (result.Succeeded)
            {
                await _context.SaveChangesAsync();
                return refreshToken;
            }

            return null;
        }

        public async Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "RefreshToken", "CurrentToken");
            return storedToken == refreshToken;
        }

        public async Task<bool> RevokeRefreshTokenAsync(string userId, string refreshToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "RefreshToken", "CurrentToken");
            if (storedToken != refreshToken)
                return false;

            var result = await _userManager.RemoveAuthenticationTokenAsync(user, "RefreshToken", "CurrentToken");
            if (result.Succeeded)
            {
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
