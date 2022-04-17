using FileExchanger.Helpers;
using FileExchanger.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace FileExchanger.Controllers
{
    [Route("api/c")]
    [ApiController]
    public class ConfirmController : ControllerBase
    {
        DbApp db;
        IMemoryCache cache;
        public ConfirmController(DbApp db, IMemoryCache memoryCache)
        {
            this.db = db;
            cache = memoryCache;
        }

        [HttpGet("e/{key}")]
        public IActionResult ConfirmEmail(string key)
        {
            AuthClientModel authClient = new AuthClientModel();
            if (!cache.TryGetValue($"EMAIL_CONFIRM_{key}", out authClient))
                return Redirect("/");

            cache.Remove($"EMAIL_CONFIRM_{key}");
            db.AuthClients.Add(authClient);
            db.Directory.Add(new DirectoryModel()
            {
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Name = "/",
                Key = "root",
                Owner = authClient
            });
            db.SaveChanges();
            Response.Cookies.Append(Config.Instance.Auth.CookiesName, JwtHelper.CreateToken(authClient.Email, authClient.Password));
            return Redirect("/");
        }
    }
}
