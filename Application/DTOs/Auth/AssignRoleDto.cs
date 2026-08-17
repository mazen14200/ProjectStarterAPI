using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Auth
{
    public class AssignRoleDto
    {
        public string UserId { get; set; } = default!;
        public string RoleName { get; set; } = default!;
    }

    // Application/Dtos/Auth/AddClaimDto.cs
    public class AddClaimDto
    {
        public string UserId { get; set; } = default!;
        public string ClaimType { get; set; } = default!;
        public string ClaimValue { get; set; } = default!;
    }
}
