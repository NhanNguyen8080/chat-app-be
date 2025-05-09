using ChatService.API.Handlers;
using Microsoft.AspNetCore.Authentication;

namespace ChatService.API.Extensions
{
    public static class AuthExtension
    {
        public static IServiceCollection AddJwtAuth(this IServiceCollection services)
        {
            services.AddAuthentication("Scheme")
                        .AddScheme<AuthenticationSchemeOptions, CustomAuthenticationHandler>("Scheme", null);
            return services;
        }
    }
}
