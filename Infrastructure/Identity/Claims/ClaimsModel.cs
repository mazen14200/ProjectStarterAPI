using Domain.Entities;

namespace Infrastructure.Identity.Claims
{
    public class ClaimsModel
    {
        public string? RoleId { get; set; }

        public List<ClaimSelection> RolesClaimsList { get; set; } = new();
        public List<ClaimSelection> UsersClaimsList { get; set; } = new();
        public List<ClaimSelection> MessagesClaimsList { get; set; } = new();
        public List<ClaimSelection> SettingsClaimsList { get; set; } = new();
    }
}
