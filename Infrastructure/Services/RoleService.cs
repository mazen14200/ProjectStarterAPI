using Application.DTOs.Role;
using Application.Interfaces.Services;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleService(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<bool> CreateRoleAsync(CreateRoleDTO roleDto)
        {
            var role = new ApplicationRole
            {
                Name = roleDto.Name,
                RoleNumber = roleDto.RoleNumber
            };

            var result = await _roleManager.CreateAsync(role);
            return result.Succeeded;
        }

        public async Task<bool> UpdateRoleAsync(UpdateRoleDTO roleDto)
        {
            var role = await _roleManager.FindByIdAsync(roleDto.Id);
            if (role == null)
                return false;

            role.Name = roleDto.Name;
            role.RoleNumber = roleDto.RoleNumber;

            var result = await _roleManager.UpdateAsync(role);
            return result.Succeeded;
        }

        public async Task<bool> SoftDeleteRoleAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return false;

            role.isDeleted = true;
            var result = await _roleManager.UpdateAsync(role);
            return result.Succeeded;
        }

        public async Task<bool> HardDeleteRoleAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return false;

            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded;
        }

        public async Task<bool> RestoreRoleAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return false;

            role.isDeleted = false;
            var result = await _roleManager.UpdateAsync(role);
            return result.Succeeded;
        }

        public async Task<List<RoleDTO>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles
                .Where(r => !r.isDeleted)
                .ToListAsync();

            return roles.Select(r => new RoleDTO
            {
                Id = r.Id,
                Name = r.Name,
                RoleNumber = r.RoleNumber
            }).ToList();
        }

        public async Task<List<RoleDTO>> GetAllRolesWithDeletedAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            return roles.Select(r => new RoleDTO
            {
                Id = r.Id,
                Name = r.Name,
                RoleNumber = r.RoleNumber,
                IsDeleted = r.isDeleted
            }).ToList();
        }

        public async Task<RoleDTO> GetRoleByIdAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null || role.isDeleted)
                return null;

            return new RoleDTO
            {
                Id = role.Id,
                Name = role.Name,
                RoleNumber = role.RoleNumber
            };
        }

        public async Task<RoleDTO> GetDeletedRoleByIdAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null || !role.isDeleted)
                return null;

            return new RoleDTO
            {
                Id = role.Id,
                Name = role.Name,
                RoleNumber = role.RoleNumber,
                IsDeleted = true
            };
        }

        public async Task<bool> RoleNameExistsAsync(string name, string excludeId = null)
        {
            var role = await _roleManager.FindByNameAsync(name);
            if (role == null)
                return false;

            if (excludeId != null && role.Id == excludeId)
                return false;

            return true;
        }

        public async Task<byte[]> ExportRolesToExcelAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            
            // Simple CSV export for now - can be enhanced with proper Excel library
            var csv = "Id,Name,RoleNumber,IsDeleted\n";
            foreach (var role in roles)
            {
                csv += $"{role.Id},{role.Name},{role.RoleNumber},{role.isDeleted}\n";
            }

            return System.Text.Encoding.UTF8.GetBytes(csv);
        }
    }
}
