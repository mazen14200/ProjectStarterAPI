using Domain.Entities;

namespace Application.DTOs
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
