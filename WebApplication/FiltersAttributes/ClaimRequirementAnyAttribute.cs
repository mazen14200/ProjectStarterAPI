using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication.Filters
{
    #region Example in Controller On Actions 

    //  [ClaimRequirementAny("AddChance", "EditChance")]
    //  public async Task<IActionResult> AddEdit(CreateChanceDto model)
    //  {
    #endregion Example in Controller On Actions 


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class ClaimRequirementAnyAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _claimTypes;

        public ClaimRequirementAnyAttribute(params string[] claimTypes)
        {
            _claimTypes = claimTypes;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!_claimTypes.Any(claimType => context.HttpContext.User.HasClaim(c => c.Type == claimType)))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
