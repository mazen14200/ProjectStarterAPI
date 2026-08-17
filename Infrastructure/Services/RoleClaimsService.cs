using Application.DTOs;
using Application.Interfaces.Services;
using Domain.Entities;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class RoleClaimsService : IRoleClaimsService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleClaimsService(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<ClaimsModel> GetClaimsForRoleAsync(int roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                return new ClaimsModel();

            var claims = await _roleManager.GetClaimsAsync(role);

            var model = new ClaimsModel
            {
                RoleId = role.Id
            };

            // Map claims to the appropriate lists based on claim type
            foreach (var claim in claims)
            {
                var claimSelection = new ClaimSelection
                {
                    ClaimType = claim.Type,
                    Label = claim.Type,
                    IsSelected = true
                };

                if (claim.Type.StartsWith("Roles"))
                    model.RolesClaimsList.Add(claimSelection);
                else if (claim.Type.StartsWith("Users"))
                    model.UsersClaimsList.Add(claimSelection);
                else if (claim.Type.StartsWith("Messages"))
                    model.MessagesClaimsList.Add(claimSelection);
                else if (claim.Type.StartsWith("Settings"))
                    model.SettingsClaimsList.Add(claimSelection);
            }

            return model;
        }

        public async Task<bool> UpdateRoleClaimsAsync(int roleId, ClaimsModel model)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                return false;

            // Get existing claims
            var existingClaims = await _roleManager.GetClaimsAsync(role);

            // Remove all existing claims
            foreach (var claim in existingClaims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            // Add new claims based on the model
            var allClaims = new List<ClaimSelection>();
            allClaims.AddRange(model.RolesClaimsList.Where(c => c.IsSelected));
            allClaims.AddRange(model.UsersClaimsList.Where(c => c.IsSelected));
            allClaims.AddRange(model.MessagesClaimsList.Where(c => c.IsSelected));
            allClaims.AddRange(model.SettingsClaimsList.Where(c => c.IsSelected));

            foreach (var claim in allClaims)
            {
                var identityClaim = new System.Security.Claims.Claim(claim.ClaimType, "true");
                var result = await _roleManager.AddClaimAsync(role, identityClaim);
                if (!result.Succeeded)
                    return false;
            }

            return true;
        }
    }
}
