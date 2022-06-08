using Core;
using Core.Helpers;
using Core.Models;
using FileExchanger.Models;
using FileExchanger.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileExchanger.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private AuthClientModel? AuthClient => db.AuthClients.SingleOrDefault(p => p.Email == User.Identity.Name);
        private UserModel GetUser => db.Users.FirstOrDefault(p => p.Key == HttpContext.Request.Cookies["u_key"]);
        private readonly DbApp db;
        private readonly IMemoryCache cache;
        public UserController(DbApp db, IMemoryCache memoryCache)
        {
            this.db = db;
            this.cache = memoryCache;
            Cleaner.ClearUsers(db);
        }

        [Authorize(Policy = "AuthExchanger")]
        [HttpPost("create")]
        public IActionResult Create()
        {
            if(AuthClient != default && AuthClient.ExchangerUser != default)
            {
                HttpContext.Response.Cookies.Append("u_key", AuthClient.ExchangerUser.Key);
                AuthClient.ExchangerUser.LastActive = DateTime.Now;
                return Ok(AuthClient.ExchangerUser);
            }
            if (GetUser != null)
            {
                GetUser.LastActive = DateTime.Now;
                db.SaveChanges();
                return Ok(GetUser);
            }
            else
                HttpContext.Response.Cookies.Delete("u_key");
            var user = new UserModel()
            {
                Key = "".RandomString(256),
                RegistrationDate = DateTime.Now,
                LastActive = DateTime.Now,
                MaxFileCount = Config.Instance.Services.FileExchanger.MaxUploadCount,
                MaxFileSize = Config.Instance.Services.FileExchanger.MaxSaveSize,
                MaxSaveFileTime = Config.Instance.Services.FileExchanger.MaxSaveTime
            };
            db.Users.Add(user);
            if(AuthClient != default)
            {
                AuthClient.ExchangerUser = user;
            }
            db.SaveChanges();
            HttpContext.Response.Cookies.Append("u_key", user.Key);
            return Ok(user);
        }

        [Authorize(Policy = "AuthExchanger")]
        [HttpGet("detect")]
        public IActionResult Detect()
        {
            if (db.Users.Any(p => p.Key == HttpContext.Request.Cookies["u_key"]))
                return Ok("User found!");
            return BadRequest("User not found!");
        }

        [Authorize(Policy = "AuthExchanger")]
        [HttpGet("my")]
        public IActionResult GetMyInfo()
        {
            if (GetUser == null)
                return Unauthorized("Are you not authorized!");
            if(AuthClient == default)
                return Ok(new
                {
                    id = GetUser.Id,
                    registrationDate = GetUser.RegistrationDate
                });
            return Ok(new
            {
                id = GetUser.Id,
                registrationDate = GetUser.RegistrationDate,
                authClient = new
                {
                    id = AuthClient.Id,
                    username = AuthClient.Name,
                    email = AuthClient.Email
                }
            });
        }

        [Authorize]
        [HttpGet("is-admin")]
        public IActionResult GetIsAdmin()
        {
            return Ok(db.Admins.Any(p => p.AuthClient == AuthClient));
        }

        [Authorize]
        [HttpPut("my")]
        public void SetUser()
        {
            string email = Request.Headers["email"];
            string username = Request.Headers["username"];
            string oldPassword = Request.Headers["oldPassword"];
            string newPassword = Request.Headers["newPassword"];
            bool isSaveChanges = false;
            if (!string.IsNullOrWhiteSpace(username) && username.Length >= 4 && username != AuthClient.Name)
            {
                AuthClient.Name = username;
                isSaveChanges = true;
                Response.StatusCode = StatusCodes.Status202Accepted;
                Response.WriteAsync("Username");
            }
            if(!string.IsNullOrWhiteSpace(oldPassword) && !string.IsNullOrWhiteSpace(newPassword) && newPassword != oldPassword)
            {
                if(AuthClient.Password == PasswordHelper.GetHash(oldPassword))
                {
                    AuthClient.Password = PasswordHelper.GetHash(newPassword);
                    isSaveChanges = true;
                }
                Response.StatusCode = StatusCodes.Status202Accepted;
                Response.WriteAsync("\nPassword");
            }
            if (!string.IsNullOrWhiteSpace(email) && !db.AuthClients.Any(p => p.Email == email))
            {
                var key = "".RandomString(96);
                cache.Set($"CONFIRM_NEW_EMAIL_{key}", $"{AuthClient.Id}|{email}", TimeSpan.FromMinutes(30));
                var host = HttpContext.Request.Host;
                EmailService.Send(AuthClient.Email, "Confirm new email!", $"https://{host.Host}:{host.Port}/api/c/n/e/{key}");
                Response.StatusCode = StatusCodes.Status200OK;
                Response.WriteAsync("\nEmail");
            }
            if(isSaveChanges)
                db.SaveChanges();
        }
    }
}
