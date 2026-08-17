using Application.DTOs;

namespace Application.Interfaces.Services
{
    public interface IIdentityService
    {
        Task<UserDto?> GetByIdAsync(string userId);
        Task<bool> CreateAsync(CreateUserDto dto);
        Task<bool> AssignRoleAsync(string userId, string role);
        Task<string?> GenerateRefreshTokenAsync(string userId);
        Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(string userId, string refreshToken);
    }
}
