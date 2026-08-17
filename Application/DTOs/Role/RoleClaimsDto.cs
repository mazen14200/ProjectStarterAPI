using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Role
{
    public class RoleClaimsDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public List<ModuleClaimsDto> ModuleClaims { get; set; } = new List<ModuleClaimsDto>();
    }
}