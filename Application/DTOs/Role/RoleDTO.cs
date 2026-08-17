namespace Application.DTOs.Role
{
    public class RoleDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int? RoleNumber { get; set; }
        public bool IsDeleted { get; set; }
    }
}
