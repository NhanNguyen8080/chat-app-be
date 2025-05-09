using Microsoft.AspNetCore.Mvc.Filters;

namespace ChatService.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RoleAuthorizeAttribute(params string[] roles) : Attribute, IAuthorizationFilter
    {
        public string[] Roles { get; } = roles;

        public void OnAuthorization(AuthorizationFilterContext context)
        {
        }
    }
}
