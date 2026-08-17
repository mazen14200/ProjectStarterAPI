using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication.Filters
{

    #region Example in Controller On Actions 

    //  [ClaimRequirement("EditCall")]
    //  public IActionResult ReturnCallToDev(int id)
    //  {
    #endregion Example in Controller On Actions 

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class ClaimRequirementAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _claimType;

        public ClaimRequirementAttribute(string claimType)
        {
            _claimType = claimType;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.HasClaim(c => c.Type == _claimType))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
