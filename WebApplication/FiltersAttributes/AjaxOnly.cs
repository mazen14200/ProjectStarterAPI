using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace WebApplication.Filters
{
    #region Example in Controller On Actions 
        //  [HttpGet]
        //  [AjaxOnly]
        //  [ClaimRequirement("EditCall")]
        //  public IActionResult ReturnCallToDev(int id)
        //  {
    #endregion Example in Controller On Actions 

    public class AjaxOnlyAttribute : ActionMethodSelectorAttribute
    {
        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            var request = routeContext.HttpContext.Request;
            var isAjax = request.Headers["x-requested-with"] == "XMLHttpRequest";

            return isAjax;
        }
    }
}