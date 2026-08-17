using Application.DTOs;

namespace Application.Interfaces.Services
{
    public interface IRoleClaimsService
    {
        Task<ClaimsModel> GetClaimsForRoleAsync(int roleId);
        Task<bool> UpdateRoleClaimsAsync(int roleId, ClaimsModel model);
    }
}