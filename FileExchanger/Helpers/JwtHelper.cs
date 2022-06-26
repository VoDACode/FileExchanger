using Core;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace FileExchanger.Helpers
{
    public static class JwtHelper
    {
        public static async Task<string> GenerateRefreshToken()
        {
            var secureRandomBytes = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            await Task.Run(() => randomNumberGenerator.GetBytes(secureRandomBytes));
            var refreshToken = Convert.ToBase64String(secureRandomBytes);
            return refreshToken;
        }
        public static async Task<string> CreateToken(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };
            ClaimsIdentity identity =
            new ClaimsIdentity(claims, Config.Instance.Auth.CookiesName, ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                    issuer: Config.Instance.Auth.Issuer,
                    audience: Config.Instance.Auth.Audience,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(Config.Instance.Auth.LifeTime),
                    signingCredentials: new SigningCredentials(Config.Instance.Auth.GetSymmetricSecurityKey, SecurityAlgorithms.HmacSha256));

            return await Task.Run(() => new JwtSecurityTokenHandler().WriteToken(jwt));
        }
    }
}
