using FileExchanger.Helpers;
using Core.Helpers;
using FileExchanger.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Core;
using Core.Models;
using System.IO;

namespace FileExchanger.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private DbApp db;
        private AuthClientModel? authClient => db.AuthClients.SingleOrDefault(p => p.Email == User.Identity.Name);
        private IMemoryCache cache;
        public AuthController(DbApp db, IMemoryCache memoryCache)
        {
            this.db = db;
            cache = memoryCache;
        }

        [HttpPost("login")]
        public void Login(string e, string p)
        {
            void incorrect()
            {
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                HttpContext.Response.WriteAsync("INCORRECT_PASSWORD_OR_EMAIL");
            }
            if (string.IsNullOrWhiteSpace(p) || string.IsNullOrWhiteSpace(e))
            {
                incorrect();
                return;
            }
            if (db.AuthClients.Any(o => o.Email == e && o.Password == p))
            {
                incorrect();
                return;
            }
            p = PasswordHelper.GetHash(p);
            var token = JwtHelper.CreateToken(e, p);
            if (token == null)
            {
                incorrect();
                return;
            }
            HttpContext.Response.Cookies.Append(Config.Instance.Auth.CookiesName, token);
            HttpContext.Response.StatusCode = StatusCodes.Status201Created;
            HttpContext.Response.WriteAsync(token);
            Console.WriteLine(JwtHelper.Verify(token));
        }
        [HttpPost("regin")]
        public void Regin()
        {
            string e = HttpContext.Request.Headers["Email"];
            string n = HttpContext.Request.Headers["Username"];
            string p = null;
            using (var reader = new StreamReader(Request.Body))
            {
                p = reader.ReadToEndAsync().Result;
            }
            if (string.IsNullOrWhiteSpace(e) || string.IsNullOrWhiteSpace(p) || string.IsNullOrWhiteSpace(n) || n.Length < 4)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                Response.WriteAsync("BAD_REQUEST");
                return;
            }
            if (db.AuthClients.Any(o => o.Email == e))
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                Response.WriteAsync("EMAIL_ADDRESS_ALREADY_BUSY");
                return;
            }
            var user = new AuthClientModel()
            {
                Email = e,
                Password = PasswordHelper.GetHash(p),
                Name = n
            };
            string confirmKey = "".RandomString(64);
            var host = HttpContext.Request.Host;
            cache.Set($"EMAIL_CONFIRM_{confirmKey}", user, DateTime.Now.AddHours(1));
            EmailService.Send(e, "Confirm email!", $"https://{host.Host}:{host.Port}/api/c/e/{confirmKey}");

            Response.StatusCode = StatusCodes.Status200OK;
            Response.WriteAsync("OK");
        }
        [Authorize]
        [HttpGet("check")]
        public void CheckAuth() => Ok();
    }
}
