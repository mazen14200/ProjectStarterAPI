using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(256)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(128, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
    }
}
