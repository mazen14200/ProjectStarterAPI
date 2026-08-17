using Domain.Enums;
using WebApplication.Attributes;
using Infrastructure.InterfacesDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;


namespace WebApplication.Helpers
{
    public class PermissionScanner
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUnitOfWork _unitOfWork;


        public PermissionScanner(IHttpContextAccessor httpContextAccessor, IAuthorizationService authorizationService, IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _authorizationService = authorizationService;
            _unitOfWork = unitOfWork;
        }

        public static List<string> GetAllActionPermissions()
        {
            var permissions = new List<string>();

            var controllers = Assembly.GetEntryAssembly()?
                .GetTypes()
                .Where(type => typeof(Controller).IsAssignableFrom(type));

            foreach (var controller in controllers!)
            {
                bool controllerHasAdminAuthorize = controller.IsDefined(typeof(AdminAuthorizeAttribute), inherit: true);
                if (controllerHasAdminAuthorize)
                {
                    var actions = controller
                     .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                     .Where(m =>
                         !m.IsDefined(typeof(NonActionAttribute), true) &&  // skip [NonAction]
                         !m.IsDefined(typeof(IgnoreActionAttribute), true) && // skip [IgnoreAction]
                         !(m.IsDefined(typeof(HttpPostAttribute), true) && m.Name == "AddEdit") // optional: keep old rule
                     );


                    var flag = true;
                    foreach (var action in actions)
                    {
                        var controllerName = controller.Name.Replace("Controller", "");
                        var actionName = action.Name;
                        if (actionName == "AddEdit" && flag)
                        {
                            permissions.Add($"{controllerName}.Add");
                            permissions.Add($"{controllerName}.Edit");
                            flag = false; // Ensure Add and Edit are only added once
                        }
                        if (actionName != "AddEdit")
                        {
                            permissions.Add($"{controllerName}.{actionName}");
                        }
                    }
                }
            }

            return permissions.OrderBy(x => x).ToList();
        }

        public bool ValidatePermission(string controller, string action)
        {
            var policyName = $"{controller}.{action}";

            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
                return false;

            var result = _authorizationService.AuthorizeAsync(user, null, policyName).Result;
            return result.Succeeded;
        }

        public RoleNumber ValidateRoleNumber()
        {
            var roleNumber = _httpContextAccessor.HttpContext?.Session.GetInt32("RoleNumber");
            if (roleNumber != null)
            {
                return (RoleNumber)roleNumber;
            }
            return RoleNumber.NormalUser;
        }

        public async Task<bool> CheckLoggedUserIfTrainer()
        {
            return await Task.FromResult(false);
        }

        public async Task<bool> CheckLoggedUserIfHasSignature()
        {
            return await Task.FromResult(false);
        }

    }

}
