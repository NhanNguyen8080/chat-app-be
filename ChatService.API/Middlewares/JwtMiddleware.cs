using ChatService.API.Attributes;
using System.Security.Claims;

namespace ChatService.API.Middlewares
{
    public class JwtMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var rolesAuthorize = endpoint?.Metadata.GetMetadata<RoleAuthorizeAttribute>();

            if (rolesAuthorize is not null)
            {
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    var userRoles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value);
                    if (!rolesAuthorize.Roles.Any(role => userRoles.Contains(role))) {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Unauthorized: You do not have permission to access this resource.");
                        return;
                    }
                } else
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Forbidden: You need to log in to access this resource.");
                    return;
                }
            }
            await _next(context);
            return;
        }
    }
}
