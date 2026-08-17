using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity.Claims
{

    public class AppUserClaimsPrincipalFactory
        : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        public AppUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            // إضافة FullName
            if (!string.IsNullOrEmpty(user.FullName))
                identity.AddClaim(new Claim("FullName", user.FullName));

            //// إضافة UserTypeId
            //if (user.UserTypeId.HasValue)
            //    identity.AddClaim(new Claim("UserTypeId", user.UserTypeId.Value.ToString()));

            return identity;
        }
    }
}
