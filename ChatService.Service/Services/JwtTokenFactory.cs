using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Service.Services
{
    public interface IJwtTokenFactory
    {
        ClaimsPrincipal? ValidToken(string token);
    }
    public class JwtTokenFactory : IJwtTokenFactory
    {
        private readonly string _secretKey;
        private readonly IConfiguration _configuration;

        public JwtTokenFactory(IConfiguration configuration)
        {
            _secretKey = Environment.GetEnvironmentVariable("JWT_SECRET") 
                            ?? throw new ArgumentNullException("JWT_SECRET environment variable is not set.");
            _configuration = configuration;
        }

        public ClaimsPrincipal? ValidToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero // Remove delay of token when expire
                }, out SecurityToken validatedToken);

                return principal;
            } catch (Exception ex)
            {
                return null;
            }
        }
    }
}
