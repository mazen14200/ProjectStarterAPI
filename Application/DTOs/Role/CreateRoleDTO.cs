using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Role
{
    public class CreateRoleDTO
    {
        [Required]
        [StringLength(256)]
        public string Name { get; set; } = string.Empty;

        public int? RoleNumber { get; set; }
    }
}
