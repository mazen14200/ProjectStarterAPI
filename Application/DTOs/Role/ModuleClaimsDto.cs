using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Role
{
    public class ModuleClaimsDto
    {
        public string ModuleName { get; set; }
        public List<ClaimSelectionDto> Claims { get; set; } = new List<ClaimSelectionDto>();
    }
}