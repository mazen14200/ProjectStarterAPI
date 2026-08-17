using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Role
{
    public class UpdateRoleDTO
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(256)]
        public string Name { get; set; } = string.Empty;

        public int? RoleNumber { get; set; }
    }
}
