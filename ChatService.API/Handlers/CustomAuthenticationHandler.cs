using ChatService.API.Attributes;
using ChatService.Service.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace ChatService.API.Handlers
{
    public class CustomAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder,
        IJwtTokenFactory jwtTokenFactory) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var endpoint = Context.GetEndpoint();
            var roleAuthorize = endpoint?.Metadata.GetMetadata<RoleAuthorizeAttribute>();

            if (roleAuthorize is null)
            {
                return AuthenticateResult.NoResult(); //skip authentication if not required
            }

            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer", StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.Fail("Token is missing");
            }

            var token = GetTokenFromHeader(authHeader);
            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.Fail("Token is missing");
            }

            var principal = jwtTokenFactory.ValidToken(token);
            if (principal is null)
            {
                return AuthenticateResult.Fail("Invalid token");
            } else
            {
                return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
            }

        }

        private string GetTokenFromHeader(string authHeader)
        {
            return authHeader.Split(' ').Last();
        }
    }
}
