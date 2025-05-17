using ChatService.Repository.Models;
using ChatService.Service.DTOs;
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
        Task<AuthenticationResult> CreateTokenAsync(Account account);
        void RevokeRefreshToken(string refreshToken);
        bool ValidateRefreshToken(string refreshToken, out string phoneNumber);
        ClaimsPrincipal? ValidToken(string token);
    }
    public class JwtTokenFactory : IJwtTokenFactory
    {
        private readonly string _secretKey;
        private readonly IAccountService _accountService;
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, string> RefreshTokens = new();

        public JwtTokenFactory(IConfiguration configuration, IAccountService accountService)
        {
            DotNetEnv.Env.Load();
            _secretKey = Environment.GetEnvironmentVariable("JWT_SECRET")
                            ?? throw new ArgumentNullException("JWT_SECRET environment variable is not set.");
            _configuration = configuration;
            _accountService = accountService;
        }

        public async Task<AuthenticationResult> CreateTokenAsync(Account account)
        {
            var roles = await _accountService.GetUserRoles(account.Id);

            var claims = new List<Claim>
            {
                new Claim("Id", account.Id.ToString()),
                new Claim("PhoneNumber", account.PhoneNumber),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(30);

            var token = new JwtSecurityToken(
                               issuer: _configuration["Jwt:Issuer"],
                               audience: _configuration["Jwt:Audience"],
                               claims: claims,
                               expires: expires,
                               signingCredentials: creds);

            var tokenHandler = new JwtSecurityTokenHandler().WriteToken(token);

            var authenticationResult = new AuthenticationResult
            {
                Token = tokenHandler,
                Expires = DateTime.Now.AddMinutes(30),
                FullName = account.FullName,
                PhoneNumber = account.PhoneNumber,
                Roles = roles,
            };
            return authenticationResult;
        }

        public void RevokeRefreshToken(string refreshToken)
        {
            RefreshTokens.Remove(refreshToken);
        }

        public bool ValidateRefreshToken(string refreshToken, out string? phoneNumber)
        {
            return RefreshTokens.TryGetValue(refreshToken, out phoneNumber);
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
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
