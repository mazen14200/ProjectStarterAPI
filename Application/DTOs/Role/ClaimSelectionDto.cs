using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Role
{
    public class ClaimSelectionDto
    {
        public string ClaimType { get; set; }
        public string Label { get; set; }
        public bool IsSelected { get; set; }
    }
}