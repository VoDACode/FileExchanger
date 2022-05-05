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

namespace FileExchanger.Helpers
{
    public static class JwtHelper
    {
        public static bool Verify(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidIssuer = Config.Instance.Auth.Issuer,
                ValidAudience = Config.Instance.Auth.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config.Instance.Auth.Key))
            };

            SecurityToken validatedToken;
            IPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            return true;
        }
        public static string CreateToken(string email, string password)
        {
            var identity = getIdentity(email, password);
            if (identity == null)
                return null;

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                    issuer: Config.Instance.Auth.Issuer,
                    audience: Config.Instance.Auth.Audience,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(Config.Instance.Auth.LifeTime),
                    signingCredentials: new SigningCredentials(Config.Instance.Auth.GetSymmetricSecurityKey, SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
        private static ClaimsIdentity getIdentity(string email, string password)
        {
            using (var db = new DbApp(Config.Instance.DbConnect))
            {
                var user = db.AuthClients.FirstOrDefault(u => u.Email == email && u.Password == password);
                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                        new Claim(ClaimsIdentity.DefaultIssuer, user.Id.ToString())
                    };
                    ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims, Config.Instance.Auth.CookiesName, ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType);
                    return claimsIdentity;
                }
                return null;
            }
        }
    }
}
